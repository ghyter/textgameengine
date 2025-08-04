using System.Text.Json.Serialization;

namespace GameModel.Models.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExitDirections
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest,
    Up,
    Down,
    Custom,

}