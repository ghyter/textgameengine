using System;

namespace GameModel.Model;

public interface IGameElement
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string StartingState { get; set; }
    public string StartingLocation { get; set; }
    public string ToDescription(string stateId);
    
}
