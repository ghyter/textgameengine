using System.Text;
using GameModel.Pack;
using GameModel.Actions;
using GameModel.Model;
using System.Linq;

namespace GameModel;

public class GameSession
{
    const string _playerid = "player:player";

    public string GameTitle { get; set; } = "Text Game Engine";

    public ActionRegistry ActionRegistry { get; set; } = new();

    public List<string> SceneOrdinals { get; set; } = [];
    public List<string> InventoryOrdinals { get; set; } = [];

    public GameElementInfo Player {get => Elements[_playerid];}

    public GameElementInfo? CurrentLocation
    {
        get
        {
            var sceneId = Player.Location ?? string.Empty;
            return Elements.TryGetValue(sceneId, out var info) ? info : null;
        }

    }

    public GameElements Elements { get; set; } = [];

    public static GameSession NewGame(string PackPath)
    {
        GameSession gs = new();

        LoadGamePack(gs, PackPath);
        StandardActions.Register(gs);

        return gs;
    }

    private static void LoadGamePack(GameSession gs, string packPath)
    {
        var _gamePack = GamePackLoader.Load(packPath);
        if (_gamePack == null)
        {
            throw new ArgumentException("Invalid game pack path or format.");
        }

        gs.GameTitle = _gamePack.Title ?? "Text Game Engine";
        gs.Elements[_playerid] = new GameElementInfo
        {

            Id = _playerid,
            Element = _gamePack.Player,
            Location = _gamePack.Player.StartingLocation,
            Attributes = _gamePack.Player.Attributes, //Load the initial state of the attributes.

        };

        foreach (var s in _gamePack.Scenes)
        {
            var id = $"scene:{s.Key}";
            gs.Elements[id] = new GameElementInfo
            {
                Id = id,
                Element = s.Value,
                Location = null,
                IsVisible = s.Value.IsVisible,
                State = s.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
            s.Value.Exits.ForEach(exit =>
            {
                exit.Id = $"exit:{s.Value.Id}:{exit.TargetId}";
                gs.Elements[exit.Id] = new()
                {
                    Id = exit.Id,
                    IsVisible = exit.IsVisible,
                    Element = exit,
                    Location = id,
                    State = exit.StartingState
                };
            });
        }

        foreach (var i in _gamePack.Items)
        {
            var id = $"item:{i.Key}";
            gs.Elements[id] = new GameElementInfo
            {
                Id = id,
                Element = i.Value,
                IsVisible = i.Value.IsVisible,
                Location = i.Value.StartingLocation,
                State = i.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
        }

        foreach (var npc in _gamePack.Npcs)
        {
            var id = $"npc:{npc.Key}";
            gs.Elements[id] = new GameElementInfo
            {
                Id = id,
                Element = npc.Value,
                IsVisible = npc.Value.IsVisible,
                Location = npc.Value.StartingLocation,
                State = npc.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
        }

        //Add the scene prefix for any element that doest start with _
        foreach (var element in gs.Elements.Values)
        {
            if (element.Location != null
                && !element.Location.StartsWith("_")
                && !element.Location.StartsWith("scene:")
              )
            {
                element.Location = $"scene:{element.Location}";
            }
        }

        //Add the data driven actions.
        foreach (var action in _gamePack.Actions)
        {
            gs.ActionRegistry.Register(action);
        }

    }


    public IGameElement? GetGameElement(string id)
    {
        return Elements.TryGetValue(id, out var info) ? info.Element : null;
    }

    public T? GetGameElement<T>(string id) where T : class, IGameElement
    {
        return Elements.TryGetValue(id, out var info) ? info.Get<T>() : null;
    }


    public string Execute(string input)
    {
        StringBuilder sb = new();
        var actionresult = ActionRegistry.TryExecute(this, input, out var result) ? result : result;

        //Header

        sb.Append(GameTitle);
        sb.Append(": ");
        var sceneName = CurrentLocation?.Get<Scene>()?.Name ?? "Unknown Scene";
        sb.AppendLine(sceneName);
        sb.AppendLine(new string('=', GameTitle.Length + sceneName.Length + 4));
        sb.AppendLine(actionresult);

        return sb.ToString();
    }

    public void PopulateOrdinals()
    {
        SceneOrdinals.Clear();
        SceneOrdinals.AddRange(Elements.Values
            .Where(e =>
                e.Id.StartsWith("exit:")
                && e.IsVisible
                && e.Location != null
                && e.Location.Equals(CurrentLocation?.Id)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));
        SceneOrdinals.AddRange(Elements.Values
            .Where(e =>
                e.Id.StartsWith("npc:")
                && e.IsVisible
                && e.Location != null
                && e.Location.Equals(CurrentLocation?.Id)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));
        SceneOrdinals.AddRange(Elements.Values
            .Where(e =>
                e.Id.StartsWith("item:")
                && e.IsVisible
                && e.Location != null
                && e.Location.Equals(CurrentLocation?.Id)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));


        InventoryOrdinals.Clear();
        InventoryOrdinals.AddRange(Elements.Values
            .Where(
                e => e.Location != null
                && e.IsVisible
                && e.Location.Equals("_inventory")
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));

    }

    public string PrintSceneOrdinals()
    {
        StringBuilder sb = new();
        int i = 0;
        if (SceneOrdinals.Any(s => s.StartsWith("exit:")))
        {
            sb.AppendLine("Exits:");
            foreach (var exitId in SceneOrdinals.Where(s => s.StartsWith("exit:")))
            {

                var exit = Elements[exitId];
                var name = exit.Element.Name ?? exitId;
                //Since this is getting the exit from the game element, the prefix hasn't been added yet.
                var targetScene = "scene:" + GetGameElement<Exit>(exitId)?.TargetId;
                //Only show the target of the exit if the target scene is visible.
                //Whem moving through an exit the first time, the scene should be set to visible.
                var scene = Elements[targetScene!];
                if (scene?.IsVisible ?? false)
                {
                    sb.AppendLine($"{++i}. {name} ({scene.Element.Name})");
                }
                else
                {
                    sb.AppendLine($"{++i}. {name}");
                }

            }
        }
        if (SceneOrdinals.Any(s => s.StartsWith("npc:")))
        {

            sb.AppendLine("Characters:");
            foreach (var npc in SceneOrdinals.Where(s => s.StartsWith("npc:")))
            {
                var npcElement = GetGameElement<Npc>(npc);
                var name = npcElement?.Name ?? npc;
                sb.AppendLine($"{++i}. {name} ({npc})");
            }
        }
        if (SceneOrdinals.Any(s => s.StartsWith("item:")))
        {
            sb.AppendLine("Items:");
            foreach (var item in SceneOrdinals.Where(s => s.StartsWith("item:")))
            {
                var itemElement = GetGameElement<Item>(item);
                var name = itemElement?.Name ?? item;
                sb.AppendLine($"{++i}. {name} ({item})");
            }
        }
        return sb.ToString();
    }

}
