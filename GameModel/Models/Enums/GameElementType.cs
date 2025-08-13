using System.Text.Json.Serialization;

namespace GameModel.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameElementType
{
    Scene,
    Npc,
    Item,
    Exit,
    Player,
    
}
