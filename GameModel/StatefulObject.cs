using System;

namespace GameModel;

public abstract class StatefulObject
{
    public Dictionary<string, string> States { get; set; } = [];
    public string DefaultState { get; set; } = "default";


}
