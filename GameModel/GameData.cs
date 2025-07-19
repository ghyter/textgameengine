namespace GameModel;

public class GameData
{
    public string StartingRoomId { get; set; } = "intro";
    public Dictionary<string, Room> Rooms { get; set; } = new();
}

public class Room
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, string> Exits { get; set; } = new();
}
