using System;

namespace GameModel;

public class MappedObject
{
    public string Id { get; set; } = "";           // e.g. "bill"
    public string Type { get; set; } = "";         // e.g. "npc", "item", "player"
    public string? LocationId { get; set; } = "";  // e.g. "hallway", "inventory", etc.

    

}
