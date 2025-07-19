using GameModel;
namespace GameModel.Actions;

public delegate string GameActionHandler(GameSession session, PlayerAction action);

public class GameActionRegistry
{
    private readonly Dictionary<string, GameActionHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);

    public void Register(GameActionHandler handler, params string[] verbs)
    {
        foreach (var verb in verbs)
            _handlers[verb] = handler;
    }

    public bool TryExecute(GameSession session, PlayerAction action, out string result)
    {
        if (_handlers.TryGetValue(action.VerbText, out var handler))
        {
            result = handler(session, action);
            return true;
        }

        result = $"I don't know how to '{action.VerbText}'.";
        return false;
    }

    public IReadOnlyDictionary<string, GameActionHandler> Handlers => _handlers;
}

