using GameModel.Model;

namespace GameModel;

public class GameElementInfo
{
    public required string Id { get; init; }
    public required IGameElement Element { get; init; }
    public string State { get; set; } = "default";
    public string? Location { get; set; }
    public List<Exit> Exits { get; set; } = [];

    public string Description => Element.ToDescription(State);

    public T? Get<T>() where T : class, IGameElement => Element as T;
}


public class GameElements : Dictionary<string, GameElementInfo>;


public static class GameElementsExtensions
{
    public static IEnumerable<GameElementInfo> GetInLocation(this GameElements map, string location, string? type = null)
    {
        return map.Values.Where(o => o.Location == location && (type == null || o.Id.StartsWith($"{type}:") ));
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
        return map.GetLocationOf(id) == "inventory";
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

