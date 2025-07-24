namespace GameModel.Model;

//Check to see if the game element is in a specific location, has a specific state, or is in the inventory
//This is used to determine if a condition is met for an action to be performed
//If the condition is met, the action can be performed
//If the location is null, the condition will not check the location.
//Likewise if the state is null, the condition will not check the state.
public class Condition
{
    public string GameElementId { get; set; } = string.Empty;
    public string? LocationId { get; set; }
    public string? StateId { get; set; }
}


public enum EffectType
{
    Location,
    State,
    HitPoints,
}

public class Effect
{
    public string GameElementId { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty; // e.g., "location", "state", "inventory"
    public string? TargetValue { get; set; }
}
