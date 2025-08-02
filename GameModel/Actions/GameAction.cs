/// <summary>
/// Represents an action that can be performed in the game, including its targets, conditions, effects, and associated messages.
/// </summary>
/// <remarks>
/// A <see cref="GameAction"/> defines the structure and requirements for an action, such as the number of required targets,
/// the canonical verb and its aliases, conditions that must be met, and effects that occur upon execution.
/// </remarks>
using GameModel.Modes.Enums;
using GameModel.Helpers;
using GameModel.Models;
namespace GameModel.Actions;




public class GameAction
{
    public required string Id { get; set; } = string.Empty;

    public int RequiredTargets { get; set; } = 0; //0,1,2
    public string Target1 { get; set; } = string.Empty;
    public string Target2 { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ActionDifficulty Difficulty { get; set; } = ActionDifficulty.Trivial;
    public string? AttributeCheck { get; set; }  //Set this to the attribute you want to use for this check.  IE Dexterity, Intellegence.
    //Note, the engine does not define the attributes, that is only in the datapack.
    public string? CanonicalVerb { get; set; }
    public List<string> VerbAliases { get; set; } = [];
    public List<Condition> Conditions { get; set; } = [];
    public List<Effect> Effects { get; set; } = [];

    public ActionHandler Handler { get; set; } = ActionHandlers.DefaultActionHandler;
    public override string ToString() => $"{Name} ({Id})";

    public string? SuccessMessage { get; set; } = string.Empty;
    public string? FailureMessage { get; set; } = string.Empty;


    public string Execute(GameSession session, PlayerAction action)
    {
        //Check if the conditions are met for this action
        if (!ConditionsMet(session, action, out var message))
        {
            //Otherwise return the message from the condition check
            return message.ResolvePlaceholders(session, action);
        }
        //The handler may be null.
        bool success = CheckSuccess(session, action, out var roll, out var bonus, out int total, out int threshold);
        Console.WriteLine($"({(success ? "success" : "failed")}) d20:{roll} bonus:{bonus} =  {total} >= {threshold}");
        

        //Call the handler to execute the action
        return Handler(session, this, action).ResolvePlaceholders(session, action);
    }

    public bool ConditionsMet(GameSession session, PlayerAction action, out string message)
    {
        message = string.Empty;
        if (Conditions.Count == 0)
            return true; // No conditions means the action is always valid

        //Return true if all conditions are met.
        foreach (var condition in Conditions)
        {
            if (!condition.IsMet(session, action, out var result))
            {
                //If any condition fails, return false
                message = result;
                return false;
            }
        }
        return true; // All conditions are met
    }


public bool CheckSuccess(GameSession session, PlayerAction action, out int roll, out int bonus, out int total, out int threshold)
{
    // Roll a d20 (or support d20a/d20d if you wish)
     string diceExpr = session.Player.RollType switch
    {
        RollType.Advantage => "d20a",
        RollType.Disadvantage => "d20d",
        _ => "d20"
    };
    roll = DiceHelper.Roll(diceExpr);
    // Get attribute bonus if present
    bonus = 0;
    if (!string.IsNullOrEmpty(AttributeCheck))
    {
        if (session.Player.Attributes.TryGetValue(AttributeCheck, out int attributeScore))
        {
            bonus = (attributeScore - 10) / 2;
        }
    }

    // (Optionally add item/gear bonuses here)

    total = roll + bonus;
    threshold = (int)Difficulty;
    if (roll == 20){ return true; }
    if (roll == 1){ return false; }
    return total >= threshold;
}

}
