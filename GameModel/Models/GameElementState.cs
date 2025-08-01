using GameModel.Models.Constants;
using GameModel.Modes.Enums;

namespace GameModel.Models;

public class GameElementState
{
    
    public required string Id { get; init; }
    public required IGameElement Element { get; init; }
    public string State { get; set; } = "default";
    public bool IsVisible { 
        get {
            if (Flags.TryGetValue("IsVisible", out bool flag))
            {
                return flag;
            }
            return true;
        }
        set => Flags["IsVisible"] = value;
     }
    public string? Location { get; set; }
    public RollType RollType { get; set; } = RollType.Regular;

    public Dictionary<string, int> Attributes { get; set; } = [];
    public Dictionary<string, string> Properties { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];

    public string Description => Element.ToDescription(State);

    public T? Get<T>() where T : class, IGameElement => Element as T;
}


public class GameElements : Dictionary<string, GameElementState>;

public static class GameElementsExtensions
{
    public static IEnumerable<GameElementState> GetInLocation(this GameElements map, string location, string? type = null)
    {
        return map.Values.Where(o => o.Location == location && o.IsVisible && (type == null || o.Id.StartsWith($"{type}:") ));
    }
    
    public static string? GetLocationOf(this GameElements map, string id)
    {
        return map.TryGetValue(id, out var info) ? info.Location : null;
    }

    public static bool IsInScene(this GameElements map, string id, string sceneId)
    {
        var loc = map.GetLocationOf(id);
        return loc == sceneId;
    }

    public static bool IsInInventory(this GameElements map, string id)
    {
        return map.GetLocationOf(id) == GameConstants.InventoryId;
    }

    public static bool IsOffMap(this GameElements map, string id)
    {
        return map.GetLocationOf(id) == "_off";
    }

    public static bool IsInPlay(this GameElements map, string id)
    {
        var loc = map.GetLocationOf(id);
        return loc != null && loc != "_off";
    }
}

