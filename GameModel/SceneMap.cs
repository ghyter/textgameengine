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
