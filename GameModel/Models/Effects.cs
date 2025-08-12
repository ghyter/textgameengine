using GameModel.Modes.Enums;
using GameModel.Helpers;
using GameModel.Session;

namespace GameModel.Models;

public class Effect
{
    public string GameElementId { get; set; } = string.Empty;
    public EffectType Type { get; set; }
    public string Property { get; set; } = string.Empty; // e.g., "location", "state", GameConstants.InventoryId
    public string? NewValue { get; set; } // e.g., "scene:hall", "state:locked", GameConstants.InventoryId
    public string? SuccessMessage { get; set;}
}
public static class EffectExtensions
{
    public static bool ApplyEffect(this GameSession session, Effect effect, GameRound round, out string result)
    {
        round.Log.Add($"Effect: {effect!.Type.ToString()}");
        if (effect == null || string.IsNullOrEmpty(effect.GameElementId))
        {
            result = string.Empty;
            round.Log.Add("No Effect");
            return false; // No effect to apply
        }

        if (session.Elements.TryGetValue(effect.GameElementId.ResolvePlaceholders(session, round.PlayerAction!), out var element))
        {
            switch (effect.Type)
            {
                case EffectType.ChangeState:
                    if (!string.IsNullOrEmpty(effect.NewValue))
                    {
                        round.Log.Add($"Change State: {element.Element.Name} from {element.State} to {effect.NewValue} ");
                        element.State = effect.NewValue;
                    }
                    break;
                case EffectType.ChangeLocation:
                    var newLocationId = effect.NewValue.ResolvePlaceholders(session, round.PlayerAction!);
                    if (!string.IsNullOrEmpty(newLocationId))
                    {
                        if (newLocationId == "_inventory")
                        {
                            element.Location = newLocationId;
                            round.Log.Add($"Moved Player from {element.Element.Name} to Inventory ");
                        }
                        else if (session.Elements.TryGetValue(newLocationId, out var newLocation))
                        {
                            element.Location = newLocation.Id;
                            round.Log.Add($"Moved Player from {element.Element.Name} to {newLocation.Element.Name} ");
                        }
                        else
                        {
                            round.Log.Add($"Could not find location {newLocationId}");
                        }
                    }
                    break;
                case EffectType.AddToInventory:
                    round.Log.Add($"Added {element.Element.Name}");
                    element.Location = "_inventory";
                    break;
                case EffectType.RemoveFromInventory:
                    round.Log.Add($"Dropped {element.Element.Name}");
                    element.Location = session.Player.Location;
                    break;
                case EffectType.SetProperty:
                    element.Properties[effect.Property] = effect.NewValue ?? string.Empty;
                    Console.WriteLine($"Setting property {effect.Property} to {effect.NewValue} on {element.Id}");
                    break;
                case EffectType.Custom:
                    // Custom logic can be implemented here
                    break;
            }
            result = effect.SuccessMessage ?? string.Empty;
            return true;
        }
        result = "Nothing to apply.";
        return false;
    }
}
