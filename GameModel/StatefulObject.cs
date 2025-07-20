using System;
using GameModel.Model;

namespace GameModel;

public abstract class StatefullGameElements: IGameElement
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // This is a dictionary of states and their descriptions
    // The default state is the one that is used if no other state is specified
    public Dictionary<string, string> States { get; set; } = [];
    public string DefaultState { get; set; } = "default";


}
