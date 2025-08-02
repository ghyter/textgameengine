using System.Text.Json.Serialization;
using GameModel.Actions;
using GameModel.Enums;
using GameModel.Helpers;

namespace GameModel.Models;


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
        var resolvedId = GameElementId.ResolvePlaceholders(session, action);
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
            ConditionRuleType.IsVisible => element.IsVisible,
            ConditionRuleType.IsMovable => element.Get<Item>()?.IsMovable ?? false,
            _ => false,
        };
        
        return PassesRules;
    }

  

private bool EvaluateInLocation(GameElementState element, GameSession session, PlayerAction action, out string result)
{
    var resolved = (Value ?? "").ResolvePlaceholders(session, action);

    var allowed = resolved.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var isValid = allowed.Contains(element.Location, StringComparer.OrdinalIgnoreCase);
    if (!isValid)
    {
        result = $"You cannot see $Target1.Name here.";
    }
    else
    {
        result = string.Empty;
    }
    return isValid;
}

}
