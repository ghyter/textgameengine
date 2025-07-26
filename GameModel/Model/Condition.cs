using System.Text.Json.Serialization;
using GameModel.Actions;

namespace GameModel.Model;

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

}


//Check to see if the game element is in a specific location, has a specific state, or is in the inventory
//This is used to determine if a condition is met for an action to be performed
//If the condition is met, the action can be performed
//If the location is null, the condition will not check the location.
//Likewise if the state is null, the condition will not check the state.
public class Condition
{
    public string GameElementId { get; set; } = string.Empty;
    public ConditionRuleType Rule { get; set; }
    public string? Target { get; set; } = string.Empty; // e.g., varies by rule.  Attribute Name for HasAttribute, command for InHistory
    public string? Comparison { get; set; } = string.Empty; // e.g., "equals", "contains", "startsWith", "endsWith"
    public string? Value { get; set; } // e.g., "scene:hall", "state:locked", "_inventory"

    public string? FailMessage { get; set; } // Optional message to display if the condition fails


    public bool IsMet(GameSession session, PlayerAction action, out string result)
    {
        var resolvedId = ResolveElementId(GameElementId, action);
        result = string.Empty;

        if (string.IsNullOrEmpty(resolvedId))
        {
            result = "Condition has no GameElementId.";
            return false;
        }

        if (!session.Elements.TryGetValue(resolvedId, out var element))
        {
            result = $"Element '{resolvedId}' not found.";
            return false;
        }


        bool PassesRules = Rule switch
        {
            ConditionRuleType.HasState => element.Element.States.ContainsKey(Value ?? string.Empty),
            ConditionRuleType.StateValue => element.State == Value,
            ConditionRuleType.InLocation => EvaluateInLocation(element, session, action,out result),
            ConditionRuleType.InHistory => false,
            ConditionRuleType.HasAttribute => false,
            ConditionRuleType.PropertyValue => false,
            ConditionRuleType.HasProperty => false,
            _ => false,
        };
        
        return PassesRules;
    }

    private static string? ResolveElementId(string? id, PlayerAction action)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return id switch
        {
            "$Player" => "player:player",
            "$Target1" => action.Targets.ElementAtOrDefault(0),
            "$Target2" => action.Targets.ElementAtOrDefault(1),
            _ => id
        };
    }

private bool EvaluateInLocation(GameElementInfo element, GameSession session, PlayerAction action, out string result)
{
    var resolved = (Value ?? "")
        .Replace("$Inventory", "_inventory")
        .Replace("$Location", session.CurrentScene?.Id ?? string.Empty);

    var allowed = resolved.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var isValid = allowed.Contains(element.Location, StringComparer.OrdinalIgnoreCase);
    if (!isValid)
    {
        result = $"You cannot see {element.Id} here.";
    }
    else
    {
        result = string.Empty;
    }
    return isValid;
}

}


public enum EffectType
{
    ChangeState, // Change the state of a GameElement
    ChangeLocation, // Move a GameElement to a new location
    AddToInventory, // Add a GameElement to the player's inventory
    RemoveFromInventory, // Remove a GameElement from the player's inventory
    SetProperty, // Set a property of a GameElement
    Custom // Custom effect defined by the game logic
}


public class Effect
{
    public string GameElementId { get; set; } = string.Empty;
    public EffectType Type { get; set; }
    public string Property { get; set; } = string.Empty; // e.g., "location", "state", "inventory"
    public string? NewValue { get; set; } // e.g., "scene:hall", "state:locked", "_inventory"
}
