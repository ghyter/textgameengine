using System;

namespace GameModel;


public enum Verbs
{
    look,
    move,
    use,
    combine,
    get,
    talk,
    unknown
}

public class PlayerAction
{
    public Verbs Verb { get; set; } = Verbs.unknown;
    public List<string> Targets { get; set; } = [];

    private static readonly Dictionary<string, Verbs> VerbAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["look"] = Verbs.look,
        ["examine"] = Verbs.look,
        ["inspect"] = Verbs.look,
        ["move"] = Verbs.move,
        ["go"] = Verbs.move,
        ["walk"] = Verbs.move,
        ["use"] = Verbs.use,
        ["combine"] = Verbs.combine,
        ["mix"] = Verbs.combine,
        ["get"] = Verbs.get,
        ["take"] = Verbs.get,
        ["pick"] = Verbs.get,
        ["talk"] = Verbs.talk,
        ["speak"] = Verbs.talk
    };

    private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

    public static PlayerAction Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new InvalidDataException("Input is empty");

        var tokens = input
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (tokens.Count == 0)
            throw new InvalidDataException("No command provided.");

        var verbToken = tokens[0].ToLowerInvariant();

        if (!VerbAliases.TryGetValue(verbToken, out var verb))
            throw new InvalidDataException($"Unknown verb: '{verbToken}'");

        var result = new PlayerAction { Verb = verb };

        var remaining = tokens.Skip(1).ToList();
        if (remaining.Count == 0) return result;

        // Detect separator (e.g. "use key on door")
        var clauseIndex = remaining.FindIndex(t => ClauseSeparators.Contains(t.ToLowerInvariant()));

        if (clauseIndex >= 0)
        {
            var before = remaining.Take(clauseIndex).ToList();
            var after = remaining.Skip(clauseIndex + 1).ToList();

            if (before.Count > 0) result.Targets.Add(string.Join(" ", before));
            if (after.Count > 0) result.Targets.Add(string.Join(" ", after));
        }
        else
        {
            result.Targets.Add(string.Join(" ", remaining));
        }

        return result;
    }
}
