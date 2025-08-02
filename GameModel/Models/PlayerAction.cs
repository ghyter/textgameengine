namespace GameModel.Models;

public class PlayerAction
{

    public string VerbText { get; set; } = default!;
    public List<string> Targets { get; set; } = [];

    public override string ToString()
    {
        var target1 = Targets.Count > 0? Targets.FirstOrDefault() : string.Empty;
        var target2 = Targets.Count > 1 ? Targets.LastOrDefault() : string.Empty;
        return $"{VerbText} {target1} {target2}";
    }

}
