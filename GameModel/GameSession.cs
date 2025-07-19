using System;

namespace GameModel;

public class GameSession
{
    public GamePack GamePack { get; set; } = default!;
    public Player Player { get; set; } = new();
    public List<PlayerAction> ActionHistory { get; set; } = [];

    public SceneMap SceneMap { get; set; } = new();

}
