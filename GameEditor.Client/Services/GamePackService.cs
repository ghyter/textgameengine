using System.ComponentModel;
using Blazor.IndexedDB;
using GameEditor.Client.Data;
using GameModel.Models;

public interface IGamePackService: INotifyPropertyChanged
{
    GamePack?      Current      { get; }
    string?        CurrentKey   { get; }

    Task InitializeAsync();

    // CRUD:
    Task<string>           CreateAsync(string id, GamePack pack);
    Task<GamePackRecord?>        ReadAsync(string id);
    Task<List<GamePackRecord>>   ReadAllAsync();
    Task<bool>             UpdateAsync(string id, GamePack pack);
    Task<bool>             DeleteAsync(string id);

    // Helpers:
    Task<bool>             SwitchAsync(string id);
}

public class GamePackService : IGamePackService
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IIndexedDbFactory _factory;
    private const string Store = nameof(GameEditorDb.GamePacks);

    public GamePack?   Current    { get; private set; }
    public string?     CurrentKey { get; private set; }

    public GamePackService(IIndexedDbFactory factory)
        => _factory = factory;

    public async Task InitializeAsync()
    {
        using var db = await _factory.Create<GameEditorDb>();
        CurrentKey = db.Settings.SingleOrDefault(s => s.Key == "currentProject")?.Value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentKey)));
        if (CurrentKey != null)
            Current = db.GamePacks.SingleOrDefault(p => p.Id == CurrentKey)?.Pack;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
    }

    public async Task<string> CreateAsync(string id, GamePack pack)
    {
        var record = new GamePackRecord { Id = id, Pack = pack };

        using var db = await _factory.Create<GameEditorDb>();
        db.GamePacks.Add(record);
        await db.SaveChanges();
        return id;
    }

    public async Task<GamePackRecord?> ReadAsync(string id)
    {
        using var db = await _factory.Create<GameEditorDb>();
        return db.GamePacks.SingleOrDefault(p => p.Id == id);
    }

    public async Task<List<GamePackRecord>> ReadAllAsync()
    {
        using var db = await _factory.Create<GameEditorDb>();
        return db.GamePacks.ToList();
    }

    public async Task<bool> UpdateAsync(string id, GamePack pack)
    {
        using var db = await _factory.Create<GameEditorDb>();
        var existing = db.GamePacks.SingleOrDefault(p => p.Id == id);
        if (existing == null) return false;

        existing.Pack = pack;
        try
        {
            await db.SaveChanges();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return true;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        using var db = await _factory.Create<GameEditorDb>();
        var existing = db.GamePacks.SingleOrDefault(p => p.Id == id);
        if (existing == null) return false;

        db.GamePacks.Remove(existing);
        await db.SaveChanges();
        return true;
    }

    public async Task<bool> SwitchAsync(string id)
    {
        if (!(await ReadAsync(id).ConfigureAwait(false) is GamePackRecord packrecord)) 
            return false;

        // Save “currentProject” setting
        using var db = await _factory.Create<GameEditorDb>();
        var old = db.Settings.SingleOrDefault(s => s.Key == "currentProject");
        if (old != null) db.Settings.Remove(old);

        db.Settings.Add(new SettingRecord { Key = "currentProject", Value = id });
        await db.SaveChanges();

        Current    = packrecord.Pack;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
        CurrentKey = id;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentKey)));
        return true;
    }
}
