using GameModel.Model;

namespace GameModel;

public class GameElementInfo
{
    public required string Type { get; init; }
    public required IGameElement Element { get; init; }
    public string State { get; set; } = "default";
    public string? LocationId { get; set; }
    public List<string> Exits { get; set; } = [];

    public T? Get<T>() where T : class, IGameElement => Element as T;
}
