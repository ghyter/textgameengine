using System.Text;
using GameModel.Pack;
using GameModel.Actions;
using GameModel.Model;

namespace GameModel;

public class GameSession
{
    private GameActionRegistry _actionRegistry { get; set; } = new();

    public GamePack GamePack { get; set; } = default!;
    public Player Player { get; set; } = new();
    public List<PlayerAction> ActionHistory { get; set; } = [];

    public SceneMap SceneMap { get; set; } = new();
    public Scene CurrentScene { get; set; } = new();

    public static GameSession NewGame(string PackPath)
    {
        var gamePack = GamePackLoader.Load(PackPath);

        GameSession gs = new();
        gs.GamePack = gamePack;
        gs.SceneMap = gamePack.InitialSceneMap;

        gs._actionRegistry.Register(Handlers.HandleLook, "look", "examine", "view");

        return gs;
    }

    public string Screen()
    {
        StringBuilder sb = new();

        //Check the scenemap for the Player's location.

        string playerlocation = SceneMap.GetLocationOf("player", "player") ?? "default";
        var scene = GamePack.Scenes[playerlocation];

        //If you have just moved to this scene, then
        //print the full description, otherwise just the name.
        //full description is available on look
        bool showDescription = false;
        if (CurrentScene == null || CurrentScene.SceneId != scene.SceneId)
        {
            CurrentScene = scene;
            showDescription = true;
        }
        sb.Append(GamePack.Title);
        sb.Append(": ");
        sb.AppendLine(scene.Name);
        sb.AppendLine("============");

        if (showDescription)
        {
            sb.AppendLine(Handlers.HandleLook(this, new()));
        }
        return sb.ToString();
    }

    public string Execute(string input)
    {
       var action = PlayerAction.Parse(input);
       return _actionRegistry.TryExecute(this, action, out var result) ? result : result;
    }



}
