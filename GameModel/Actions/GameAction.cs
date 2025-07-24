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

    public ActionHandler? Handler { get; set; }
    public override string ToString() => $"{Name} ({Id})";

    public string? SuccessMessage { get; set; } = "You successfully perform the action.";
    public string? FailureMessage { get; set; } = "You fail to perform the action";

}
