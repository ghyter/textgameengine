using GameModel.Modes.Enums;

namespace GameModel.Models;

public class EffectGroup
{
    public EffectGroupType Type { get; set; }
    public List<Effect> Effects { get; set; } = [];

}
