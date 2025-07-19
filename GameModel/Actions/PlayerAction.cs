namespace GameModel.Actions;

public class PlayerAction
{
    public string VerbText { get; set; } = default!;
    public List<string> Targets { get; set; } = [];

    private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

    public static PlayerAction Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new InvalidDataException("Input is empty.");

        var tokens = input
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        if (tokens.Count == 0)
            throw new InvalidDataException("No command provided.");

        var verb = tokens[0].ToLowerInvariant();
        var action = new PlayerAction { VerbText = verb };

        var remaining = tokens.Skip(1).ToList();
        if (remaining.Count == 0) return action;

        var clauseIndex = remaining.FindIndex(t => ClauseSeparators.Contains(t.ToLowerInvariant()));

        if (clauseIndex >= 0)
        {
            var before = remaining.Take(clauseIndex).ToList();
            var after = remaining.Skip(clauseIndex + 1).ToList();

            if (before.Count > 0) action.Targets.Add(string.Join(" ", before));
            if (after.Count > 0) action.Targets.Add(string.Join(" ", after));
        }
        else
        {
            action.Targets.Add(string.Join(" ", remaining));
        }

        return action;
    }
}
