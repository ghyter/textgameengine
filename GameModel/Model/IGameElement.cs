using System;

namespace GameModel.Model;

public interface IGameElement
{

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public string DefaultState { get; set; }
}
