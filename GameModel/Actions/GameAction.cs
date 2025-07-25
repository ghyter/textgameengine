/// <summary>
/// Represents an action that can be performed in the game, including its targets, conditions, effects, and associated messages.
/// </summary>
/// <remarks>
/// A <see cref="GameAction"/> defines the structure and requirements for an action, such as the number of required targets,
/// the canonical verb and its aliases, conditions that must be met, and effects that occur upon execution.
/// </remarks>
using GameModel.Model;
namespace GameModel.Actions;

public class GameAction
{
    public required string Id { get; set; } = string.Empty;

    public int RequiredTargets { get; set; } = 0; //0,1,2
    public string Target1 { get; set; } = string.Empty;
    public string Target2 { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }

    public string? CanonicalVerb { get; set; }
    public List<string> VerbAliases { get; set; } = [];
    public List<Condition> Conditions { get; set; } = [];
    public List<Effect> Effects { get; set; } = [];

    public ActionHandler Handler { get; set; } = ActionHandlers.HandleDataAction;
    public override string ToString() => $"{Name} ({Id})";

    public string? SuccessMessage { get; set; } = "You successfully perform the action.";
    public string? FailureMessage { get; set; } = "You fail to perform the action";


    public string Execute(GameSession session, PlayerAction action)
    {
        //Check if the conditions are met for this action
        if (!ConditionsMet(session, action))
        {
            return FailureMessage ?? "You cannot perform this action due to unmet conditions.";
        }

        //The handler may be null.
        //Call the handler to execute the action
        return Handler(session, this, action);
    }

    public bool ConditionsMet(GameSession session, PlayerAction action)
    {
        GameElementInfo? target1 = action.Targets.Count > 0 ? session.Elements[action.Targets[0]] : null;
        GameElementInfo? target2 = action.Targets.Count > 1 ? session.Elements[action.Targets[2]] : null;

        if (Conditions.Count == 0)
            return true; // No conditions means the action is always valid

        //Check if the conditions are met for this action
        foreach (var condition in Conditions)
        {
            var gameElement = session.Elements[condition.GameElementId];
            if (gameElement == null)
            {
                return false; // Game element not found
            }

            // Check the property type and perform the appropriate comparison
            switch (condition.Property)
            {
                case "location":
                    if (gameElement.Location == null || !gameElement.Location.Equals(condition.Value, StringComparison.OrdinalIgnoreCase))
                        return false;
                    break;
                case "state":
                    if (!gameElement.State.Equals(condition.Value, StringComparison.OrdinalIgnoreCase))
                        return false;
                    break;
                case "inventory":
                    if (!gameElement?.Location?.Equals("_inventory", StringComparison.OrdinalIgnoreCase) ?? false)
                        return false;
                    break;
                default:
                    return false; // Unknown property type
            }
        }
        return true; // All conditions met
    }

}
