using System.Text.Json.Serialization;

namespace GameModel.Modes.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConditionRuleType
{
    HasState, // Checks if the GameElement has a specific state
    StateValue, // Checks if the GameElement is in a specific state
    InLocation, // Checks if the GameElement is in a specific location
    InHistory, // Checks if the GameElement has been in a specific location or state in the past
    HasAttribute, // Checks if the GameElement has a specific attribute
    PropertyValue,
    HasProperty,
    IsVisible,
    IsMovable,

}
