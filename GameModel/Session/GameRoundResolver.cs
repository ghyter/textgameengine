using System;
using System.Text.RegularExpressions;
using GameModel.Actions;
using GameModel.Helpers;
using GameModel.Models;
using GameModel.Models.Constants;
using GameModel.Modes.Enums;

namespace GameModel.Session;

public static class GameRoundResolver
{
    public static GameRound Execute(GameSession session, string input)
    {
        GameRound round = new(input);
        round.Log.Add($"Starting Round for {input}");
        round.Body = "Round Resolution pending";
        round.Header = session.Header;

        //Parse and Resolve the input into a PlayerAction.        
        round.PlayerAction = ParsePlayerAction(session, input)?.ResolveTargets(session);
        if (round.PlayerAction == null)
        {
            round.Log.Add($"No verbs matched");
            round.Outcome = GameRound.RoundOutcome.Invalid;
            round.Body = "I don't understand your request";
            return round;
        }
        round.Log.Add($"Parsed and Resovled: {round.PlayerAction.ToString()} ");

        //Find the action that will be used.       
        round.GameAction = FindAction(session, round);
        if (round.GameAction == null)
        {
            round.Outcome = GameRound.RoundOutcome.Invalid;
            round.Log.Add($"No action matches this verb and target combination");
            round.Body = $"I don't know how to '{round.PlayerAction.ToString()}'";
            return round;
        }
        round.Log.Add($"GameAction Located: {round.GameAction.ToCommandString()}");

        //Now that we have a proper action, we will see if we have passed all the conditions
        //to resolve this.

        var conditions = round.GameAction.Conditions;
        var unmetConditions = conditions.Select(c =>
            {
                var passed = c.IsMet(session, round.PlayerAction, out var result);
                round.Log.Add($"Evaluate Condition {c.ToString()} ({passed})");
                return passed ? null : result;
            })
            .Where(msg => !string.IsNullOrEmpty(msg))
            .ToList();
        if (unmetConditions.Count > 0)
        {

            round.Outcome = GameRound.RoundOutcome.Failure;
            round.Log.Add($"No action matches this verb and target combination");
            unmetConditions.ForEach(umc => round.Log.Add($"-- {umc}"));
            round.Body = unmetConditions.First();
            return round;
        }

        //Maybe this should be in a handler method that checks regular difficulty. 
        
        //Run the difficulty checks on action.
        if (!EvaluateDifficultyCheck(session, round, out var roleResult))
        {
            round.Log.Add(roleResult.ToString());
            round.Outcome = GameRound.RoundOutcome.Failure;
            round.Body = $"{round.GameAction.FailureMessage} ({roleResult.ToString()}";
            return round;
        }
        round.Log.Add(roleResult.ToString());


        //Success, run the effects!
        //Now run the resolver.  (Soon to change)
        var handler = ActionHandlers.GetHandler(round.GameAction.EffectHandler);
        round.Body =  handler(session, round).ResolvePlaceholders(session, round.PlayerAction);

        //Now run any triggers that occur following effects.


        round.Header = session.Header;
        round.Outcome = GameRound.RoundOutcome.Success;
        session.PopulateOrdinals();
        session.GameLog.Add(round);
        return round;
    }

    public static PlayerAction? ParsePlayerAction(GameSession session, string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var normalized = input.Trim();

        // Gather all (verb, canonicalVerb) pairs, including aliases, with longest first
        var verbPairs = session.ActionRegistry.Actions
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
            var pattern = $@"^{Regex.Escape(alias!)}(\s|$)";
            //if (normalized.StartsWith(alias!, StringComparison.OrdinalIgnoreCase))
            if (Regex.IsMatch(normalized, pattern, RegexOptions.IgnoreCase))
            {
                var remainder = normalized.Substring(alias!.Length).Trim();

                var action = new PlayerAction
                {
                    VerbText = canonical!, // Always the canonical verb!
                };

                if (!string.IsNullOrWhiteSpace(remainder))
                {
                    var tokens = remainder.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                    var clauseIndex = tokens.FindIndex(t => GameConstants.ClauseSeparators.Contains(t.ToLowerInvariant()));
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


    public static PlayerAction ResolveTargets(this PlayerAction action, GameSession session)
    {
        PlayerAction results = new()
        {
            VerbText = action.VerbText,
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

    public static GameAction? FindAction(GameSession session, GameRound round)
    {
        var candidates = session.ActionRegistry.Actions
            .Where(a => string.Equals(a.CanonicalVerb, round.PlayerAction!.VerbText, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var scored = candidates
            .Select(action => new
            {
                Action = action,
                Score = MatchActionScore(action, round.PlayerAction!),
                ConditionsMet = action.Conditions?.Count(c => c.IsMet(session, round.PlayerAction!, out _)) ?? 0
            });


        var result = scored
        .Where(x => x.Score >= 0)
        .OrderByDescending(x => x.Score)
        .ThenByDescending(x => x.ConditionsMet)
        .FirstOrDefault();

        return result?.Action;
    }

    private static int MatchActionScore(GameAction action, PlayerAction playerAction)
    {
        // First, ensure the player provided the expected number of targets
        if (action.RequiredTargets != playerAction.Targets.Count)
        {
            return -1;
        }

        string target1 = playerAction.Targets.ElementAtOrDefault(0) ?? "";
        string target2 = playerAction.Targets.ElementAtOrDefault(1) ?? "";

        // Disqualify if the action doesn't support a given position
        if (playerAction.Targets.Count == 1 && string.IsNullOrEmpty(action.Target1))
        {
            return -1;
        }

        if (playerAction.Targets.Count == 2
            && (
                string.IsNullOrEmpty(action.Target1)
                || string.IsNullOrEmpty(action.Target2)
                )
            )
        {
            return -1;
        }

        int score1 = MatchTargetScore(action.Target1, target1);
        int score2 = MatchTargetScore(action.Target2, target2);

        return score1 + score2;
    }

    private static int MatchTargetScore(string? pattern, string actual)
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

    
    public static bool EvaluateDifficultyCheck(GameSession session, GameRound round, out RollResult rollResult)
    {
        rollResult = new();

        // Roll a d20 (or support d20a/d20d if you wish)
        string diceExpr = session.Player.RollType switch
        {
            RollType.Advantage => "d20a",
            RollType.Disadvantage => "d20d",
            _ => "d20"
        };
        rollResult.DiceExpression = diceExpr;
        rollResult.Roll = DiceHelper.Roll(diceExpr);
        // Get attribute bonus if present
        rollResult.Bonus = 0;
        if (!string.IsNullOrEmpty(round.GameAction?.AttributeCheck))
        {
            if (session.Player.Attributes.TryGetValue(round.GameAction.AttributeCheck, out int attributeScore))
            {
                rollResult.Bonus = (attributeScore - 10) / 2;
            }
        }

        // (Optionally add item/gear bonuses here)

        rollResult.Total = rollResult.Roll + rollResult.Bonus;
        rollResult.Threshold = (int)round.GameAction!.Difficulty;
        if (rollResult.Roll == 20) { return true; }
        if (rollResult.Roll == 1) { return false; }
        return rollResult.Total >= rollResult.Threshold;
    }

}
