using System;
using System.Text;
using GameModel.Model;

namespace GameModel.Actions;

public static class Handlers
{
    public static string HandleLook(GameSession session, PlayerAction action)
    {
        if (action.Targets.Any())
        {
            return "Not yet supported";
        }
        else
        {
            return LookScene(session, action, session.CurrentScene);
        }

    }
    
    private static string LookScene(GameSession session, PlayerAction action, Scene scene)
    {
        StringBuilder sb = new();
            //I am looking at the scene
        sb.AppendLine(scene.Description);
        var whoishere = session.SceneMap.GetInLocation(scene.SceneId);

        var npcs = whoishere.Where(x => x.Type == "npc");
        if (npcs.Any())
        {
            sb.AppendLine("The following people are present.");
            foreach (var item in npcs)
            {
                var npc = session.GamePack.Npcs[item.Id];
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
            foreach (var exit in scene.Exits)
            {
                sb.AppendLine($"* {session.GamePack.Scenes[exit].Name} ({exit})");
            }
        }

        return sb.ToString();
    }

}
