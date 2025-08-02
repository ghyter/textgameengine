using GameModel.Enums;

namespace GameModel.Model;

public class EffectGroup
{
    public EffectGroupType Type { get; set; }
    public List<Effect> Effects { get; set; } = [];

}
