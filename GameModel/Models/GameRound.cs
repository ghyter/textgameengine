using System;

namespace GameModel.Models;
/// <summary>
/// This class will hold all of details about a singluar round.
///  
/// </summary>
public class GameRound
{
    private readonly string _playerInput;
    public GameRound(string input)
    {
        _playerInput = input;
    }    

    public enum RoundOutcome { Success, Failure, Invalid }
    public RoundOutcome Outcome { get; set; } = RoundOutcome.Invalid;

    //Header after action is performed.
    public string Header { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    //The raw string that the player input
    public string PlayerInput { get => _playerInput; }

    //The parsed PlayerAction that we will act on.
    public PlayerAction? PlayerAction { get; set; }

    //The identified game action that will be performed.
    //There should only be one GameAction performed for the Verb
    public GameAction? GameAction { get; set; }

    //This will list any events triggered after our action.
    public List<GameAction> TriggeredEvents { get; set; } = [];

    //This is the final string for the Round.
    public string? Body { get; set; }

    //This is to log actions that will not be returned to the user, but may be dumped out in a log command.
    public List<string> Log { get; set; } = new();

}
