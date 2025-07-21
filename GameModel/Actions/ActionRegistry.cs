using GameModel;
namespace GameModel.Actions;

public delegate string ActionHandler(GameSession session, PlayerAction action);

public class ActionRegistry
{
    private readonly Dictionary<string, ActionHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);

    private readonly List<string> _canonicalVerbs = [];

    public void Register(ActionHandler handler, params string[] verbs)
    {
        //First verb is the canonical verb.
        _canonicalVerbs.Add(verbs[0]);
        foreach (var verb in verbs)
            _handlers[verb] = handler;
    }

    public bool TryExecute(GameSession session, string input, out string result)
    {
        var action = Parse(input);
        if (action != null && _handlers.TryGetValue(action.VerbText, out var handler))
        {
            result = handler(session, action);
            return true;
        }

        result = $"I don't know how to '{input}'.";
        return false;
    }

    /// <summary>
    /// Parses the input into a PlayerAction, matching the longest registered verb first.
    /// </summary>
    private PlayerAction? Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var normalized = input.Trim();

        // Match the longest registered verb first
        var sortedVerbs = _handlers.Keys
            .OrderByDescending(v => v.Length)
            .ToList();

        foreach (var verb in sortedVerbs)
        {
            if (normalized.StartsWith(verb, StringComparison.OrdinalIgnoreCase))
            {
                var remainder = normalized.Substring(verb.Length).Trim();

                var action = new PlayerAction
                {
                    VerbText = verb,
                    RawInput = input
                };

                if (string.IsNullOrWhiteSpace(remainder))
                    return action;

                var tokens = remainder.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

                var clauseIndex = tokens.FindIndex(t => ClauseSeparators.Contains(t.ToLowerInvariant()));
                if (clauseIndex >= 0)
                {
                    var before = tokens.Take(clauseIndex).ToList();
                    var after = tokens.Skip(clauseIndex + 1).ToList();

                    if (before.Count > 0) action.Targets.Add(string.Join(" ", before));
                    if (after.Count > 0) action.Targets.Add(string.Join(" ", after));
                }
                else
                {
                    action.Targets.Add(string.Join(" ", tokens));
                }

                return action;
            }
        }

        return null; // No matching verb found
    }

    public IReadOnlyDictionary<string, ActionHandler> Handlers => _handlers;
    private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

}

