using GameModel;
using GameModel.Model;

namespace GameModel.Actions;

public delegate string ActionHandler(GameSession session, GameAction gameaction, PlayerAction action);

public class ActionRegistry
{
    private List<GameAction> _actions = [];
    public IReadOnlyList<GameAction> Actions => _actions;

    private readonly List<PlayerAction> _history = [];
    public IReadOnlyList<PlayerAction> History => _history;



    public void Register(GameAction action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        if (string.IsNullOrWhiteSpace(action.Id)) throw new ArgumentException("Action must have a valid Id.", nameof(action));
        _actions.Add(action);
    }

    public bool TryExecute(GameSession session, string input, out string result)
    {
        var action = Parse(input);
        if (action == null)
        {
            result = $"I don't know how to '{input}'.";
            return false;
        }
    
        var verbCandidates = _actions
            .Where(a => a.CanonicalVerb?.Equals(action.VerbText, StringComparison.OrdinalIgnoreCase) == true ||
                        a.VerbAliases.Any(alias => alias.Equals(action.VerbText, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        //Check the verbCandidates to see if the conditions are met.

        result = $"I don't know how to '{input}'.";
        return false;
    }


public PlayerAction? Parse(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return null;

    var normalized = input.Trim();

    // Gather all (verb, canonicalVerb) pairs, including aliases, with longest first
    var verbPairs = _actions
        .SelectMany(a =>
            new[] { a.CanonicalVerb }
                .Concat(a.VerbAliases ?? [])
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(alias => (Alias: alias, Canonical: a.CanonicalVerb ?? alias))
        )
        .OrderByDescending(p => p.Alias?.Length)
        .ToList();

    foreach (var (alias, canonical) in verbPairs)
    {
        
        if (normalized.StartsWith(alias! , StringComparison.OrdinalIgnoreCase))
        {
            var remainder = normalized.Substring(alias!.Length).Trim();

            var action = new PlayerAction
            {
                VerbText = canonical!, // Always the canonical verb!
                RawInput = input
            };

            if (!string.IsNullOrWhiteSpace(remainder))
            {
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
                    if (tokens.Count > 0) action.Targets.Add(tokens[0]);
                    if (tokens.Count > 1) action.Targets.Add(string.Join(" ", tokens.Skip(1)));
                }

                // Only support up to 2 targets
                while (action.Targets.Count > 2)
                    action.Targets.RemoveAt(2);
            }

            return action;
        }
    }

    return null; // No matching verb found
}

    public void ResolveTargets(GameSession session, PlayerAction action)
    {
        

    }

    
    private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

}

