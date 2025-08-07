using Blazor.IndexedDB;
using System.ComponentModel.DataAnnotations;
using Microsoft.JSInterop;
using GameModel.Models;
using GameModel.Session;

namespace GameEditor.Client.Data;
public class GameEditorDb : IndexedDb
{
  public GameEditorDb(IJSRuntime js, string name = "GameEditorDb", int version = 1)
    : base(js, name, version)
  {
  }

  // store one or more GamePackRecords keyed by Id
  public IndexedSet<GamePackRecord> GamePacks { get; set; } = default!;
  public IndexedSet<GameSessionRecord> GameSessions { get; set; } = default!;
  public IndexedSet<SettingRecord> Settings { get; set; } = default!;
}

public class GamePackRecord
{
    [Key]
    public string Id { get; set; } = default!;      // e.g. "current"
    public GamePack Pack { get; set; } = new();
}


public class GameSessionRecord
{
    [Key]
    public string SlotId { get; set; } = default!;    // e.g. "slot1", "quick", timestamped GUID…
    public GameSession Session { get; set; } = default!;
}

public class SettingRecord
{
    [Key]
    public string Key   { get; set; } = default!;   // e.g. "currentProject"
    public string Value { get; set; } = default!;   // e.g. "project42"
}