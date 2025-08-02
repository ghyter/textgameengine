using System.Text.Json.Serialization;

namespace GameModel.Modes.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EffectGroupType
{
    All,
    FirstMatch,
    RandomChoice
}