using System.Text;

namespace GameModel.Models;

public abstract class GameElement : IGameElement
{
    public virtual string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public bool IsVisible { get; set; } = true;

    // This is a dictionary of states and their descriptions
    // The default state is the one that is used if no other state is specified
    public Dictionary<string, int> Attributes { get; set; } = [];
    public Dictionary<string, string> Properties { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];
    public Dictionary<string, string> States { get; set; } = [];
    public string StartingState { get; set; } = "default";
    public string StartingLocation {get; set; } = "default";

    /// <summary>
    /// Returns the description for a given state (provided externally).
    /// </summary>
    public virtual string ToDescription(string stateId)
    {
        StringBuilder sb = new();
        if (Description != string.Empty)
        {
            sb.AppendLine(Description);
            sb.AppendLine(" ");
        }
        if (States != null && States.TryGetValue(stateId, out var desc))
        {
            sb.Append(desc);
        }
        return sb.ToString();
    }
    
}
