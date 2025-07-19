using System;

namespace GameModel;

public class Npc: StatefulObject
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public int HitPoints { get; set; } = 10;
    public int ArmorClass { get; set; } = 10;
    
}
