using System;
using System.Text;
using GameModel.Pack;

namespace GameModel;

public class GameSession
{
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
            sb.AppendLine(LookScene(CurrentScene));
        }
        return sb.ToString();
    }

    public string Execute(string input)
    {
        try
        {
            var action = PlayerAction.Parse(input);
            ActionHistory.Append(action);
            return action.Verb switch
            {
                Verbs.look => Look(action),
                _ => "I don't understand that command"
            };
        }
        catch
        {
            return "I don't understand that command";
        }
    }

    private string Look(PlayerAction? action = null)
    {
        action = action ?? new();
        StringBuilder sb = new();

        //I am looking at a specific thing.
        if (action.Targets.Any())
        {
            sb.AppendLine("Look at object, not yet supported");
        }
        else
        {
            sb.AppendLine(LookScene(CurrentScene));    
        }
        
        return sb.ToString();
    }

    private string LookScene(Scene scene)
    {
        StringBuilder sb = new();
            //I am looking at the scene
        sb.AppendLine(scene.Description);
        var whoishere = SceneMap.GetInLocation(scene.SceneId);

        var npcs = whoishere.Where(x => x.Type == "npc");
        if (npcs.Any())
        {
            sb.AppendLine("The following people are present.");
            foreach (var item in npcs)
            {
                var npc = GamePack.Npcs[item.Id];
                sb.AppendLine($"* {npc.Name}");
            }
        }

        if (scene.Exits.Count == 0)
        {
            sb.AppendLine("There are no visible exits");
        }
        else
        {
            sb.AppendLine("Visible Exits:");
            foreach (var exit in CurrentScene.Exits)
            {
                sb.AppendLine($"* {GamePack.Scenes[exit].Name} ({exit})");
            }
        }

        return sb.ToString();
    }

}
