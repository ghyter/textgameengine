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
    public static bool ApplyEffect(this GameSession session, Effect effect, PlayerAction action, out string result)
    {
        if (effect == null || string.IsNullOrEmpty(effect.GameElementId))
        {
            result = string.Empty;
            return false; // No effect to apply
        }

        if (session.Elements.TryGetValue(effect.GameElementId.ResolvePlaceholders(session, action), out var element))
        {
            switch (effect.Type)
            {
                case EffectType.ChangeState:
                    if (!string.IsNullOrEmpty(effect.NewValue))
                    {
                        element.Element.States[effect.Property] = effect.NewValue;

                    }
                    break;
                case EffectType.ChangeLocation:
                    if (!string.IsNullOrEmpty(effect.NewValue))
                    {
                        element.Location = effect.NewValue.ResolvePlaceholders(session, action);
                    }
                    break;
                case EffectType.AddToInventory:
                    session.InventoryOrdinals.Add(element.Id);
                    break;
                case EffectType.RemoveFromInventory:
                    session.InventoryOrdinals.Remove(element.Id);
                    break;
                case EffectType.SetProperty:
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
