namespace GameModel.Models;

public class GamePack
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Player Player { get; set; } = new();
    public Dictionary<string, Player> Players{ get; set; } = new();
    public Dictionary<string, Scene> Scenes { get; set; } = new();
    public Dictionary<string, Item> Items { get; set; } = new();
    public Dictionary<string, Npc> Npcs { get; set; } = new();
    
    public List<GameAction> Actions { get; set; } = [];

}
