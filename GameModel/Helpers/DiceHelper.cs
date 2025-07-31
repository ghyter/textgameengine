using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace GameModel.Helpers
{
    public static class DiceHelper
    {
        // Pattern: [number]d[sides][a|d]?
        // Examples: d6, 2d8a, 4d4d, 3d12
        private static readonly Regex DiceRegex = new(@"^(\d*)d(\d+)([ad])?$", RegexOptions.Compiled);

        /// <summary>
        /// Rolls dice per statement (e.g., "d6", "2d20a", "4d4d").
        /// Trailing 'a' = advantage, 'd' = disadvantage.
        /// </summary>
        public static int Roll(string dice)
        {
            if (TryParse(dice, out var count, out var sides, out var advType))
            {
                if (advType == "a") // Advantage
                {
                    int first = RollDice(count, sides);
                    int second = RollDice(count, sides);
                    return Math.Max(first, second);
                }
                if (advType == "d") // Disadvantage
                {
                    int first = RollDice(count, sides);
                    int second = RollDice(count, sides);
                    return Math.Min(first, second);
                }
                // Normal roll
                return RollDice(count, sides);
            }
            throw new ArgumentException($"Invalid dice expression: {dice}");
        }

        public static List<int> RollAll(string dice)
        {
            if (TryParse(dice, out var count, out var sides, out var advType))
            {
                // For All, just return the first roll (even for adv/dis)
                return RollAllDice(count, sides);
            }
            throw new ArgumentException($"Invalid dice expression: {dice}");
        }

        private static bool TryParse(string dice, out int count, out int sides, out string advType)
        {
            var match = DiceRegex.Match(dice.Trim().ToLower());
            if (match.Success)
            {
                count = string.IsNullOrEmpty(match.Groups[1].Value) ? 1 : int.Parse(match.Groups[1].Value);
                sides = int.Parse(match.Groups[2].Value);
                advType = match.Groups[3].Success ? match.Groups[3].Value : "";
                return count > 0 && sides > 0;
            }
            count = 0; sides = 0; advType = "";
            return false;
        }

        private static int RollDice(int count, int sides)
        {
            var sum = 0;
            var rng = Random.Shared;
            for (int i = 0; i < count; i++)
                sum += rng.Next(1, sides + 1);
            return sum;
        }

        private static List<int> RollAllDice(int count, int sides)
        {
            var results = new List<int>();
            var rng = Random.Shared;
            for (int i = 0; i < count; i++)
                results.Add(rng.Next(1, sides + 1));
            return results;
        }
    }
}
