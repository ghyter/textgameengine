
using System.Text.Json.Serialization;

namespace GameModel.Modes.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RollType
{
    Disadvantage,
    Regular,
    Advantage
}