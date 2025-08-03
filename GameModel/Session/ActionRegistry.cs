using System.Text.RegularExpressions;
using GameModel.Models;

namespace GameModel.Session;

public delegate string ActionHandler(GameSession session, GameRound round);

public class ActionRegistry
{

    private List<GameAction> _actions = [];
    public IReadOnlyList<GameAction> Actions => _actions.AsReadOnly();

    public void Register(GameAction action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        if (string.IsNullOrWhiteSpace(action.Id)) throw new ArgumentException("Action must have a valid Id.", nameof(action));
        _actions.Add(action);
    }


}

