using System;

namespace GameModel;

public class GamePack
{
    public Dictionary<string, Scene> Scenes { get; set; } = new();
    public Dictionary<string, GameItem> Items { get; set; } = new();
    public Dictionary<string, Npc> Npcs { get; set; } = new();

    public SceneMap InitialSceneMap { get; set; } = new();


}
