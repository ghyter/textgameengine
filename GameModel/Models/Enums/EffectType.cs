using System.Text.Json.Serialization;

namespace GameModel.Modes.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EffectType
{
    ChangeState, // Change the state of a GameElement
    ChangeLocation, // Move a GameElement to a new location
    AddToInventory, // Add a GameElement to the player's inventory
    RemoveFromInventory, // Remove a GameElement from the player's inventory
    SetProperty, // Set a property of a GameElement
    SetAttribute,
    IncrementAttribute, // Set a property of a GameElement
    DecrementAttribute, // Set a property of a GameElement
    SetFlag,
    Custom // Custom effect defined by the game logic
}


