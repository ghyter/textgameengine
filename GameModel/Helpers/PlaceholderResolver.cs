using GameModel;
using GameModel.Actions;
using GameModel.Model;
using System.Text.RegularExpressions;

namespace GameModel.Helpers;

public static class PlaceholderResolver
{
    private static readonly Regex PlaceholderPattern = new(@"\$(Player|Target1|Target2|CurrentScene|Inventory|Location)", RegexOptions.Compiled);

    public static string ResolvePlaceholders(this string? input, GameSession session, PlayerAction action)
    {
        if (string.IsNullOrEmpty(input)) return "";

        var results = PlaceholderPattern.Replace(input, match =>
        {
         
            return match.Value switch
            {
                "$Player" => "player:player",
                "$Inventory" => "_inventory",
                "$Target1" => action.Targets.ElementAtOrDefault(0) ?? string.Empty,
                "$Target2" => action.Targets.ElementAtOrDefault(1) ?? string.Empty,
                "$CurrentScene" => session.CurrentScene?.Id ?? string.Empty,
                "$Location" => session.CurrentScene?.Id ?? string.Empty,
                _ => match.Value
            };
        });
        return results;
    }
}
