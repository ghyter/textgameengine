using System;
using System.Collections.Immutable;

namespace GameModel.Models.Constants;

public class GameConstants
{
    public const string PlayerId = "player:player";
    public const string InventoryId = "_inventory";

    public const string NPCPrefix = "npc";
    public const string ScenePrefix = "scene";
    public const string ItemPrefix = "item";
    public const string ExitPrefix = "exit";

    public static readonly ImmutableArray<string> ClauseSeparators = ImmutableArray.Create("on", "with", "in", "into");

    
}
