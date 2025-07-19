namespace GameModel.Model;

public class Npc: StatefullGameElements
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int HitPoints { get; set; } = 10;
    public int ArmorClass { get; set; } = 10;
    
}
