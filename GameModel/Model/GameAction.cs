namespace GameModel.Model;

public class GameAction
{
    public string Id { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;

    public string? CanonicalVerb { get; set; } = null;
    public List<string> Aliases = [];
    public List<Condition> Conditions { get; set; } = [];
    public List<Effect> Effects { get; set; } = [];
    
    public override string ToString() => $"{Name} ({Id})";

}
