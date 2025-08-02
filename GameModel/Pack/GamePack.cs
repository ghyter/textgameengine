using GameModel.Models;
using GameModel.Actions;
namespace GameModel.Pack;

public class GamePack
{
    public string Title { get; set; } = string.Empty;
    public Player Player { get; set; } = new();
    public Dictionary<string, Scene> Scenes { get; set; } = new();
    public Dictionary<string, Item> Items { get; set; } = new();
    public Dictionary<string, Npc> Npcs { get; set; } = new();
    
    public List<GameAction> Actions { get; set; } = [];

}
