using GameModel;
using GameModel.Actions;
using GameModel.Model;
using System.Text.RegularExpressions;

namespace GameModel.Helpers;

public static class PlaceholderResolver
{
    private static readonly Regex PlaceholderPattern = new(@"\$(Player|Target1|Target2|CurrentLocation|Inventory|Location)(.Name)?", RegexOptions.Compiled);

    private static string ResolveTargetLocation(string input, GameSession session)
    {
        var targetLocation = session.Elements.GetValueOrDefault(input)?.Location ?? string.Empty;
        var results = session.Elements.GetValueOrDefault(targetLocation);
        return results!.Element.Name;
    }

    public static string ResolvePlaceholders(this string? input, GameSession session, PlayerAction action)
    {
        if (string.IsNullOrEmpty(input)) return "";

        var results = PlaceholderPattern.Replace(input, match =>
        {

            return match.Value switch
            {
                "$Player.Name" => session.Elements.GetValueOrDefault("player:player")?.Element.Name ?? string.Empty,
                "$CurrentLocation.Name" => session.Elements.GetValueOrDefault(session.CurrentLocation?.Id ?? string.Empty)?.Element.Name ?? string.Empty,
                "$Target1.Name" => session.Elements.GetValueOrDefault(action.Targets.ElementAtOrDefault(0) ?? string.Empty)?.Element.Name ?? string.Empty,
                "$Target2.Name" => session.Elements.GetValueOrDefault(action.Targets.ElementAtOrDefault(1) ?? string.Empty)?.Element.Name ?? string.Empty,
                "$Player" => "player:player",
                "$Inventory" => "_inventory",
                "$Target1" => action.Targets.ElementAtOrDefault(0) ?? string.Empty,
                "$Target2" => action.Targets.ElementAtOrDefault(1) ?? string.Empty,
                "$CurrentLocation" => session.CurrentLocation?.Id ?? string.Empty,
                _ => match.Value
            };
        });
        return results;
    }
}
