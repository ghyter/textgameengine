using System.Text.Json.Serialization;

namespace GameModel.Models.Enums;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EffectHandlers
{
    DefaultActionHandler,
    HandleLook,
    HandleInventory,
    HandleMove,
    HandleDebug,
        

}
