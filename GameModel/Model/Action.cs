namespace GameModel.Model;

public class ConditionalGameAction
{
    public string Id { get; set; } = string.Empty;
    public string TargetId { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public List<Condition> Conditions { get; set; } = [];
    public List<Effect> Effects { get; set; } = [];
    
    public override string ToString() => $"{Name} ({Id})";

}
