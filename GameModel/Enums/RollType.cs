
using System.Text.Json.Serialization;

namespace GameModel.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RollType
{
    Disadvantage,
    Regular,
    Advantage
}