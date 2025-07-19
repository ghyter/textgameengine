using System;

namespace GameModel;

public class SceneMap
{
    public List<MappedObject> Objects { get; set; } = new();

    public void SetLocation(string type, string id, string? location)
    {
        var obj = Objects.FirstOrDefault(o => o.Id == id && o.Type == type);
        if (obj == null)
        {
            Objects.Add(new MappedObject { Id = id, Type = type, LocationId = location });
        }
        else
        {
            obj.LocationId = location;
        }
    }

    public IEnumerable<MappedObject> GetInLocation(string location, string? type = null)
    {
        return Objects.Where(o => o.LocationId == location && (type == null || o.Type == type));
    }
}

public static class SceneMapExtensions
{
    public static string? GetLocationOf(this SceneMap map, string type, string id)
    {
        return map.Objects
            .FirstOrDefault(o => o.Type == type && o.Id == id)
            ?.LocationId;
    }

    public static bool IsInScene(this SceneMap map, string type, string id, string sceneId)
    {
        var loc = map.GetLocationOf(type, id);
        return loc == sceneId;
    }

    public static bool IsInInventory(this SceneMap map, string type, string id)
    {
        return map.GetLocationOf(type, id) == "inventory";
    }

    public static bool IsOffMap(this SceneMap map, string type, string id)
    {
        return map.GetLocationOf(type, id) == "_off";
    }

    public static bool IsInPlay(this SceneMap map, string type, string id)
    {
        var loc = map.GetLocationOf(type, id);
        return loc != null && loc != "_off";
    }
}