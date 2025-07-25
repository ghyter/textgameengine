using System.Text.RegularExpressions;
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
        //Parse the incoming input into a PlayerAction
        var action = Parse(input);

        if (action == null)
        {
            result = $"I don't know how to '{input}'.";
            return false;
        }
        //Call resolve to update the targets to ids.
        action = ResolveTargets(session, action);
    
        GameAction? gameAction = FindAction(session, action);

        result = gameAction?.Execute(session, action) ?? $"I don't know how to '{input}'.";
        
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

            if (normalized.StartsWith(alias!, StringComparison.OrdinalIgnoreCase))
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


    public PlayerAction ResolveTargets(GameSession session, PlayerAction action)
    {
        PlayerAction results = new()
        {
            VerbText = action.VerbText,
            RawInput = action.RawInput,
            Targets = []
        };

        foreach (var target in action.Targets)
        {
            //If the target is already a full id, we are done.
            if (session.Elements.ContainsKey(target)) { results.Targets.Add(target); continue; }

            //Check to see if the target matches (prefix):{id}
            if (target.Contains(':'))
            {
                var parts = target.Split(':', 2);
                if (parts.Length == 2 && session.Elements.TryGetValue(parts[1], out var elementInfo))
                {

                    results.Targets.Add(elementInfo.Id);
                    continue;
                }
            }
            //If the value is an ordinal, we can resolve it from the SceneOrdinals or InventoryOrdinals.
            //Remove the I prefix and parse the ordinal position.  Get the Id from the corresponding list index.
            if (Regex.IsMatch(target, @"^I\d+$"))
            {
                var index = int.Parse(target.Substring(1)) - 1; // Convert to 0-based index
                if (index >= 0 && index < session.InventoryOrdinals.Count)
                {
                    results.Targets.Add(session.InventoryOrdinals[index]);
                    continue;
                }
            }
            else if (int.TryParse(target, out var ordinalIndex))
            {
                ordinalIndex--; // Convert to 0-based index
                if (ordinalIndex >= 0 && ordinalIndex < session.SceneOrdinals.Count)
                {

                    results.Targets.Add(session.SceneOrdinals[ordinalIndex]);
                    continue;
                }
            }
            //If we reach here, the target is not a valid id or ordinal.
            //We will leave it unchanged.
            //This allows for invalid ordinals to be passed through.
            //This is useful for user input that may not match any known elements.
            //This allows for user input to be flexible.
            results.Targets.Add(target);

        }
        return results;
    }

    public GameAction? FindAction(GameSession session, PlayerAction action)
    {
        //Find the action that matches the verb and targets.
        var matchingAction = _actions.FirstOrDefault(a =>
            a.CanonicalVerb?.Equals(action.VerbText, StringComparison.OrdinalIgnoreCase) == true ||
            a.VerbAliases.Any(alias => alias.Equals(action.VerbText, StringComparison.OrdinalIgnoreCase)));

        return matchingAction;
    }


    private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

}

