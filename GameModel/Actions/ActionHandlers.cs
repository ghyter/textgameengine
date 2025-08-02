using System;
using System.Text;
using GameModel.Models;
using GameModel.Models.Constants;

namespace GameModel.Actions;

public static class ActionHandlers
{

    public static string DefaultActionHandler(GameSession session, GameAction gameaction, PlayerAction action)
    {
        //By the time we get here, conditions have been met.
        //It is time to apply the effects.
        StringBuilder sb = new();

        gameaction.Effects.ForEach(ef => {
             if (session.ApplyEffect(ef, action, out var result))
            {
                if (result != null && result != string.Empty)
                sb.AppendLine(result);
            }
            ;
        });

        sb.AppendLine(gameaction.SuccessMessage);
        return sb.ToString();
    }

    public static string HandleLook(GameSession session, GameAction gameaction, PlayerAction action)
    {
        //It will only route here if there is 1 target.
        if (action.Targets.Any())
        {
            var targetId = action.Targets[0];
            if (!session.Elements.TryGetValue(targetId, out var target))
            {
                return $"I don't see that item here.";
            }
            return $"{target.Description} ({target.Id})";
        }
        else
        {
            //This is the scene description.
            StringBuilder sb = new();
            sb.AppendLine(session.CurrentLocation?.Description);
            session.PopulateOrdinals();
            sb.AppendLine(session.PrintSceneOrdinals());
            return sb.ToString();
        }
    }

    public static string HandleInventory(GameSession session, GameAction gameaction, PlayerAction action)
    {
        //This is the scene description.
        StringBuilder sb = new();
        session.PopulateOrdinals();

        var i = 0;
        foreach (var id in session.InventoryOrdinals)
        {
            if (session.Elements.TryGetValue(id, out var element))
            {
                sb.AppendLine($"I{++i} {element.Element.Name} ({element.Id})");
            }
        }
        return sb.ToString();
    }




    public static string HandleMove(GameSession session, GameAction gameaction, PlayerAction action)
    {
        var exit = action.Targets[0];
        var player = session.Elements[GameConstants.PlayerId];
        if (session.Elements.TryGetValue(exit, out GameElementState? exitElement))
        {
            if (exitElement == null)
            {
                return "Cannot find that exit";
            }

            var targetExit = exitElement.Get<Exit>();
            //Find the target scene.
            if (session.Elements.TryGetValue("scene:"+targetExit!.TargetId, out GameElementState? targetScene))
            {
                targetScene.IsVisible = true;
                player.Location = targetScene.Id;
            }
        }

        return HandleLook(session,gameaction,new PlayerAction(){VerbText = "Look"});

    }
   
    public static string HandleHistory(GameSession session, GameAction gameaction, PlayerAction action)
    {
        StringBuilder sb = new();
        sb.AppendLine("Action History:");

        if (session.ActionRegistry.History.Count == 0)
        {
            sb.AppendLine("No actions have been taken yet.");
        }
        else
        {
            for (int i = 0; i < session.ActionRegistry.History.Count; i++)
            {
                var act = session.ActionRegistry.History[i];
                sb.AppendLine($"{i + 1}: {act.VerbText} {string.Join(" ", act.Targets)}");
            }
        }
        return sb.ToString();

    }
}
