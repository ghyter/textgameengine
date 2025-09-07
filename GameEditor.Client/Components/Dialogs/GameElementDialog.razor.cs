using Blazor.Diagrams.Core.Events;
using GameEditor.Client.Services;
using GameModel.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GameEditor.Client.Components.Dialogs;

public partial class GameElementDialog : ComponentBase
{
    // -------- Query parameters --------
    [Parameter]
    [SupplyParameterFromQuery(Name = "id")]
    public string? QId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "mode")]
    public string? QMode { get; set; }


    private EditContext? _ec;
    // -------- Services / DI --------
    [Inject] public IGamePackService GamePackService { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;
    [Inject] public DialogService DialogService { get; set; } = default!;
    [Inject] public NotificationService Notifications { get; set; } = default!;

    private GamePack Pack => GamePackService.Current!;

    // -------- UI state --------
    protected string? _error;
    protected bool _isDirty;
    protected bool _isNew;

    protected string _mode = "edit";          // normalized mode
    protected string _modeDisplay => _mode.ToUpperInvariant();

    protected string _type = "scene";         // scene | item | npc | player
    protected string _typeTitle = "Scene";
    protected string _key = "";
    protected string _title = "Edit";

    // Active working copy & original
    protected IGameElement? _edit;
    protected IGameElement? _original;


    protected record SceneDropdownRecord(string Id, string Value);
    protected List<SceneDropdownRecord> SceneDropdown= [];

    private string IdPattern => @"^[a-z0-9_-]+$";
        //_type == "exit"
        //? @"^exit:[a-z0-9_-]+(?::[a-z0-9_-]+)*->[a-z0-9_-]+(?::[a-z0-9_-]+)*$"
        //: @"^(player|scene|item|exit|npc):[a-z0-9_-]+$";


    private Dictionary<string, string> StatesProxy
    {
        get
        {
            if (_edit == null)
                return new();

            if (_edit.States == null)
                _edit.States = new();

            return _edit.States;
        }
        set
        {
            if (_edit != null)
            {
                _edit.States = value;
                _isDirty = true;
            }
        }
    }

    private Dictionary<string, int> AttributesProxy
    {
        get
        {
            if (_edit == null)
                return new();

            if (_edit.Attributes == null)
                _edit.Attributes = new();

            return _edit.Attributes;
        }
        set
        {
            if (_edit != null)
            {
                _edit.Attributes = value;
                _isDirty = true;
            }
        }
    }

    private Dictionary<string, bool> FlagsProxy
    {
        get
        {
            if (_edit == null)
                return new();

            if (_edit.Flags == null)
                _edit.Flags = new();

            return _edit.Flags;
        }
        set
        {
            if (_edit != null)
            {
                _edit.Flags = value;
                _isDirty = true;
            }
        }
    }

    private Dictionary<string, string> PropertiesProxy
    {
        get
        {
            if (_edit == null)
                return new();

            if (_edit.Properties == null)
                _edit.Properties = new();

            return _edit.Properties;
        }
        set
        {
            if (_edit != null)
            {
                _edit.Properties = value;
                _isDirty = true;
            }
        }
    }



    // -------- Lifecycle --------
    protected override Task OnParametersSetAsync()
    {
        try
        {
            _error = null;
            ParseQuery();
            Load();
            _title = _mode switch
            {
                "new" => $"New {_typeTitle} · {_key}",
                "clone" => $"Clone {_typeTitle} · {_key}",
                _ => $"Edit {_typeTitle} · {_key}"
            };
            RebuildEditContext();
        }
        catch (Exception ex)
        {
            _error = ex.Message;
        }
        return Task.CompletedTask;
    }


    private void RebuildEditContext()
    {
        if (_edit is null) return;

        if (_ec != null)
            _ec.OnFieldChanged -= OnFieldChanged; // unhook old

        _ec = new EditContext(_edit);
        _ec.OnFieldChanged += OnFieldChanged;
        _isDirty = false; // fresh baseline
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        // Flip when *any* field in _edit changes
        _isDirty = _ec?.IsModified() ?? false;
        StateHasChanged();
    }


    private void ParseQuery()
    {
        _mode = (QMode ?? "edit").Trim().ToLowerInvariant();

        var id = (QId ?? "").Trim();
        if (string.IsNullOrWhiteSpace(id) || !id.Contains(':'))
            throw new InvalidOperationException("Missing or invalid id. Expected '?id={type}:{key}'.");

        var parts = id.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        _type = parts[0].Trim().ToLowerInvariant();
        _key = parts[1].Trim();

        _typeTitle = _type switch
        {
            "scene" => "Scene",
            "item" => "Item",
            "npc" => "NPC",
            "player" => "Player",
            _ => throw new InvalidOperationException($"Unknown type '{_type}'.")
        };

        if (_mode is not ("new" or "edit" or "clone")) _mode = "edit";
        _isNew = _mode is "new" or "clone";
    }

    private void Load()
    {

        SceneDropdown = [];
        SceneDropdown.Add(new("_none", "-None"));
        SceneDropdown.Add(new("_inventory", "-Inventory"));
        foreach (var scene in GamePackService.Current!.Scenes)
        {
            SceneDropdown.Add(new(scene.Value.Id,scene.Value.Name));

        }

        switch (_type)
        {
            case "scene": LoadTyped<Scene>(_key); break;
            case "item": LoadTyped<Item>(_key); break;
            case "npc": LoadTyped<Npc>(_key); break;
            case "player": LoadTyped<Player>(_key); break;
            default: throw new InvalidOperationException($"Unknown type '{_type}'.");
        }
    }

    private void LoadTyped<T>(string key) where T : class, IGameElement, new()
    {
        var src = GetFromPack<T>(_type, key.Replace($"{_type}:",string.Empty));

        if (_mode == "new")
        {
            _original = null;
            _edit = new T
            {
                Id = key,
                Name = ToTitle(key),
                Description = "",
                IsVisible = true,
                States = new(),
                Attributes = new(),
                Flags = new(),
                Properties = new()
            };
        }
        else if (_mode == "clone")
        {
            if (src is null) throw new InvalidOperationException($"{_typeTitle}:{key} not found to clone.");
            _original = src;
            _edit = DeepClone(src);
            key = key + "-copy"; // new id from query
            _isDirty = true;
        }
        else // edit
        {
            if (src is null) throw new InvalidOperationException($"{_typeTitle}:{key} not found.");
            _original = src;
            _edit = DeepClone(src);
        }
    }

   

    // -------- Save / Delete / Cancel --------
    protected async Task SaveAsync()
    {
        if (_edit is null) return;

        if (string.IsNullOrWhiteSpace(_key) || !Regex.IsMatch(_key, IdPattern))
        {
            Notifications.Notify(NotificationSeverity.Warning, "Invalid Id",
                "Use lowercase letters, digits, hyphens only.");
            return;
        }

        _edit.Id = _key;

        switch (_type)
        {
            case "scene":
                UpsertInto(Pack.Scenes, (Scene)_edit, _original as Scene);
                break;
            case "item":
                UpsertInto(Pack.Items, (Item)_edit, _original as Item);
                break;
            case "npc":
                UpsertInto(Pack.Npcs, (Npc)_edit, _original as Npc);
                break;
            case "player":
                UpsertInto(Pack.Players, (Player)_edit, _original as Player);
                break;
            default:
                throw new InvalidOperationException($"Unknown type '{_type}'.");
        }
        await GamePackService.UpdateCurrentAsync();

        RebuildEditContext();
        CloseOrNavigate(_edit);
    }

    protected Task DeleteAsync()
    {
        if (_isNew || string.IsNullOrEmpty(_key)) return Task.CompletedTask;

        switch (_type)
        {
            case "scene": Pack.Scenes.Remove(_key); break;
            case "item": Pack.Items.Remove(_key); break;
            case "npc": Pack.Npcs.Remove(_key); break;
            case "player":
                var isMain = string.Equals(_key, "main", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(_key, "player", StringComparison.OrdinalIgnoreCase);
                if (isMain)
                {
                    Notifications.Notify(NotificationSeverity.Warning, "Protected",
                        "The main Player cannot be deleted here.");
                    return Task.CompletedTask;
                }
                Pack.Players.Remove(_key);
                break;
        }

        CloseOrNavigate(result: null, deleted: true);
        return Task.CompletedTask;
    }

    protected void Cancel()
    {
        if (_isDirty)
            Notifications.Notify(NotificationSeverity.Info, "Canceled", "Your changes were not saved.");
        RebuildEditContext();
        CloseOrNavigate(null);
    }

    private void CloseOrNavigate(IGameElement? result, bool deleted = false)
    {
        try
        {
            DialogService.CloseSide(result);
            return;
        }
        catch
        {
            // not in a dialog, fall through to navigate
        }

        Nav.NavigateTo("/editor");
    }

    // -------- Pack helpers --------
    private static void UpsertInto<T>(Dictionary<string, T> dict, T updated, T? original)
        where T : class, IGameElement
    {
        var newKey = updated.Id;

        // handle rename (remove old key)
        if (original is not null && !string.Equals(original.Id, newKey, StringComparison.Ordinal))
        {
            dict.Remove(original.Id);
        }

        dict[newKey] = DeepClone(updated);
    }

    private T? GetFromPack<T>(string type, string key) where T : class, IGameElement
        => type switch
        {
            "scene" => Pack.Scenes.TryGetValue(key, out var s) ? s as T : null,
            "item" => Pack.Items.TryGetValue(key, out var i) ? i as T : null,
            "npc" => Pack.Npcs.TryGetValue(key, out var n) ? n as T : null,
            "player" => Pack.Players.TryGetValue(key, out var n) ? n as T : null,
            _ => null
        };

    private static string ToTitle(string key)
        => string.Join(' ', key.Split('-', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => char.ToUpperInvariant(s[0]) + s[1..]));

    private static T DeepClone<T>(T obj)
        => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj))!;


}
