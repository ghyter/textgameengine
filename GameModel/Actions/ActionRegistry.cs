using System.Text.RegularExpressions;
using GameModel;
using GameModel.Model;

namespace GameModel.Actions;

public delegate string ActionHandler(GameSession session, GameAction gameaction, PlayerAction action);

public class ActionRegistry
{
    private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

    private List<GameAction> _actions = [];
    public IReadOnlyList<GameAction> Actions => _actions;

    private List<PlayerAction> _history = [];
    public List<PlayerAction> History => _history;



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

        session.ActionRegistry.History.Add(action);

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
            string? resolved = null;

            // 1. Direct match
            if (session.Elements.ContainsKey(target))
            {
                resolved = target;
            }

            // 2. Prefix format: item:flashlight
            else if (target.Contains(':'))
            {
                var parts = target.Split(':', 2);
                if (parts.Length == 2 && session.Elements.TryGetValue(parts[1], out var elementInfo))
                {
                    resolved = elementInfo.Id; // resolved as "item:flashlight"
                }
            }

            // 3. Inventory ordinal
            else if (Regex.IsMatch(target, @"^I\d+$"))
            {
                var index = int.Parse(target.Substring(1)) - 1;
                if (index >= 0 && index < session.InventoryOrdinals.Count)
                {
                    resolved = session.InventoryOrdinals[index]; // correct id
                }
            }

            // 4. Scene ordinal
            else if (int.TryParse(target, out var ordinalIndex))
            {
                ordinalIndex--; // Convert to 0-based
                if (ordinalIndex >= 0 && ordinalIndex < session.SceneOrdinals.Count)
                {
                    resolved = session.SceneOrdinals[ordinalIndex];
                }
            }

            // 5. Fallback — unresolved input (preserve raw)
            results.Targets.Add(resolved ?? target);
        }

        return results;
    }

    public GameAction? FindAction(GameSession session, PlayerAction playerAction)
    {
        var candidates = _actions
            .Where(a => string.Equals(a.CanonicalVerb, playerAction.VerbText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var scored = candidates
            .Select(action => new
            {
                Action = action,
                Score = MatchActionScore(action, playerAction),
                //ConditionsMet = action.Conditions?.Count(c => c.IsMet(session, playerAction)) ?? 0
            });

        Console.WriteLine(scored.Any());

        var result = scored
        .Where(x => x.Score >= 0)
        .OrderByDescending(x => x.Score)
        //.ThenByDescending(x => x.ConditionsMet)
        .FirstOrDefault();

        return result?.Action;
    }

    private int MatchActionScore(GameAction action, PlayerAction playerAction)
    {
        // First, ensure the player provided the expected number of targets
        if (action.RequiredTargets != playerAction.Targets.Count)
        {
            return -1;
        }

        string target1 = playerAction.Targets.ElementAtOrDefault(0) ?? "";
        string target2 = playerAction.Targets.ElementAtOrDefault(1) ?? "";

        // Disqualify if the action doesn't support a given position
        if (playerAction.Targets.Count >= 1 && string.IsNullOrEmpty(action.Target1))
        {
            return -1;
        }

        if (playerAction.Targets.Count >= 2 && string.IsNullOrEmpty(action.Target2))
        {
            return -1;
        }

        int score1 = MatchTargetScore(action.Target1, target1);
        int score2 = MatchTargetScore(action.Target2, target2);

        return score1 + score2;
    }

    private int MatchTargetScore(string? pattern, string actual)
    {
        if (string.IsNullOrEmpty(pattern))
            return string.IsNullOrEmpty(actual) ? 4 : -1;

        if (pattern == "*") return 2;

        if (pattern.EndsWith(":*"))
        {
            var typePrefix = pattern[..^1]; // Remove '*'
            return actual.StartsWith(typePrefix, StringComparison.OrdinalIgnoreCase) ? 3 : -1;
        }

        return string.Equals(pattern, actual, StringComparison.OrdinalIgnoreCase) ? 4 : -1;
    }
}

