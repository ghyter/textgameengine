using Blazor.IndexedDB;
using GameEditor.Client.Data;
using GameModel.Models;
using Microsoft.JSInterop;

public interface IGamePackService
{
    GamePack      Current     { get; set; }
    string?       CurrentKey  { get; }
    
    /// <summary>Call once at app startup to wire up the DB and load the current pack (if any).</summary>
    Task InitializeAsync();
    
    /// <summary>Returns the list of available pack keys.</summary>
    Task<string[]> ListPacksAsync();
    
    /// <summary>Create a new blank pack with an optional title, returns its key.</summary>
    Task<string> CreateNewPackAsync(string? title = null);
    
    /// <summary>Save the current pack under its CurrentKey (or throws if none).</summary>
    Task SaveCurrentPackAsync();
    
    /// <summary>Switch to an existing pack by key (loads it into Current).</summary>
    Task<bool> SwitchPackAsync(string key);
}

public class GamePackService : IGamePackService
{
    private readonly IIndexedDbFactory _factory;

    public GamePack  Current    { get; set; } = new GamePack();
    public string?   CurrentKey { get; private set; }

    public GamePackService(IJSRuntime js, IIndexedDbFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        // 1) Create (and open) the DB
        using var db = await _factory.Create<GameEditorDb>();

        // 2) Load “currentProject” from Settings store
        var settings = db.Settings.ToArray();
        CurrentKey   = settings.FirstOrDefault(s => s.Key == "currentProject")?.Value;

        // 3) If there is a key, load that pack
        if (CurrentKey != null)
        {
            var packs = db.GamePacks.ToArray();
            var rec   = packs.SingleOrDefault(p => p.Id == CurrentKey);
            if (rec != null)
                Current = rec.Pack;
        }
    }

    public async Task<string[]> ListPacksAsync()
    {
        using var db = await _factory.Create<GameEditorDb>();
        return db.GamePacks
                 .Select(p => p.Id)
                 .ToArray();
    }

    public async Task<string> CreateNewPackAsync(string? title = null)
    {
        var key = Guid.NewGuid().ToString("N");
        var record = new GamePackRecord {
            Id   = key,
            Pack = new GamePack { Title = title ?? "New GamePack" }
        };

        using (var db = await _factory.Create<GameEditorDb>())
        {
            db.GamePacks.Add(record);
            await db.SaveChanges();           // Persist the new pack
        }

        // Switch to it
        await SwitchPackAsync(key);
        return key;
    }

    public async Task SaveCurrentPackAsync()
    {
        if (CurrentKey == null)
            throw new InvalidOperationException("No pack selected.");

        using var db = await _factory.Create<GameEditorDb>();

        // Remove the old record
        var existing = db.GamePacks.Single(p => p.Id == CurrentKey);
        db.GamePacks.Remove(existing);

        // Add the updated one
        db.GamePacks.Add(new GamePackRecord {
            Id   = CurrentKey,
            Pack = Current
        });

        await db.SaveChanges();             // Commit
    }

    public async Task<bool> SwitchPackAsync(string key)
    {
        using var db = await _factory.Create<GameEditorDb>();

        var rec = db.GamePacks.SingleOrDefault(p => p.Id == key);
        if (rec == null) return false;

        // Load into memory
        Current    = rec.Pack;
        CurrentKey = key;

        // Persist “currentProject” setting
        var old = db.Settings.SingleOrDefault(s => s.Key == "currentProject");
        if (old != null)
            db.Settings.Remove(old);

        db.Settings.Add(new SettingRecord {
            Key   = "currentProject",
            Value = key
        });

        await db.SaveChanges();             // Commit
        return true;
    }
}