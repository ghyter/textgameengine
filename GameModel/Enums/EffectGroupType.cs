using System.Text.Json.Serialization;

namespace GameModel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EffectGroupType
{
    All,
    FirstMatch,
    RandomChoice
}