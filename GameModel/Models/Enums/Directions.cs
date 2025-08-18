using System.Text.Json.Serialization;

namespace GameModel.Models.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExitDirections
{
    North = 0,
    NorthEast = 1,
    East = 2,
    SouthEast = 3,
    South = 4,
    SouthWest = 5,
    West = 6,
    NorthWest = 7,
    Up = 10,
    Down = 11,
    Custom = 99,

}


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExitLevels
{
    Same, 
    Up,
    Down,
}