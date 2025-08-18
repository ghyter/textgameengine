namespace GameModel.Models;

public class Scene : GameElement
{
    public List<Exit> Exits { get; set; } = [];
    public MapLocation MapLocation { get; set; } = new();

}
