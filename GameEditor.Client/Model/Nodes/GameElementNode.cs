using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using GameModel.Models;

namespace GameEditor.Client.Model.Nodes;

public class GameElementNode : NodeModel
{
    public GameElementNode(Point position, IGameElement element) : base(position) {
        Element = element;
    }

    public GameElementNode(string id, Point position, IGameElement element) : base(id,position)
    {
        Element = element;
    }

    public IGameElement Element { get; set; }
}

public class SceneNode : GameElementNode 
{
    public SceneNode(Point position, IGameElement element) : base(position, element) { }
    public SceneNode(string id, Point position, IGameElement element) : base(id, position, element) { }
}