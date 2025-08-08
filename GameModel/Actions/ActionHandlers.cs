using System;
using System.Text;
using GameModel.Models;
using GameModel.Models.Constants;
using GameModel.Models.Enums;
using GameModel.Session;

namespace GameModel.Actions;



public static class ActionHandlers
{
    public static ActionHandler GetHandler(EffectHandlers effect)
    {
        return effect switch
        {   EffectHandlers.HandleDebug => HandleDebug,
            EffectHandlers.HandleLook => HandleLook,
            EffectHandlers.HandleInventory => HandleInventory,
            EffectHandlers.HandleMove => HandleMove,
            _ => DefaultActionHandler
        };
    }

    public static string DefaultActionHandler(GameSession session, GameRound round)
    {
        //By the time we get here, conditions have been met.
        //It is time to apply the effects.
        StringBuilder sb = new();
        round.Log.Add("Default Action Handler");

        round.GameAction!.Effects.ForEach(ef =>
        {
            if (session.ApplyEffect(ef, round, out var result))
            {
                if (result != null && result != string.Empty)
                    sb.AppendLine(result);
            }
            ;
        });

        sb.AppendLine(round.GameAction.SuccessMessage);
        return sb.ToString();
    }

    public static string HandleLook(GameSession session, GameRound round)
    {
        //It will only route here if there is 1 target.
        if (round.PlayerAction!.Targets.Any())
        {
            var targetId = round.PlayerAction!.Targets[0];
            if (!session.Elements.TryGetValue(targetId, out var target))
            {
                return $"I don't see that item here.";
            }
            return $"{target.Description}";
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

    public static string HandleInventory(GameSession session, GameRound round)
    {
        //This is the scene description.
        StringBuilder sb = new();
        session.PopulateOrdinals();

        var i = 0;
        foreach (var id in session.InventoryOrdinals)
        {
            if (session.Elements.TryGetValue(id, out var element))
            {
                sb.AppendLine($"I{++i} {element.Element.Name}");
            }
        }
        return sb.ToString();
    }




    public static string HandleMove(GameSession session, GameRound round)
    {
        var exit = round.PlayerAction!.Targets[0];
        var player = session.Elements[GameConstants.PlayerId];
        if (session.Elements.TryGetValue(exit, out GameElementState? exitElement))
        {
            if (exitElement == null)
            {
                return "Cannot find that exit";
            }

            var targetExit = exitElement.Get<Exit>();
            //Find the target scene.
            if (session.Elements.TryGetValue("scene:" + targetExit!.TargetId, out GameElementState? targetScene))
            {
                targetScene.IsVisible = true;
                player.Location = targetScene.Id;
            }
        }


        return session.Execute("look").Body ?? string.Empty;

    }

    public static string HandleDebug(GameSession session, GameRound round)
    {
        StringBuilder sb = new();

        session.GameLog.ForEach(l =>
        {
            sb.AppendLine("--------------------------");
            sb.AppendLine($"Player Input: {l.PlayerInput}");
            sb.AppendLine($"Player Action: {l.PlayerAction?.ToString()}");
            sb.AppendLine($"Game Action: {l.GameAction?.ToCommandString()}");
            l.Log.ForEach(ll => sb.AppendLine("--" + ll));
            sb.AppendLine($"Outcome: {l.Outcome.ToString()}");
        });
        return sb.ToString();
    }


}
