using System;
using System.Text;
using GameModel.Model;

namespace GameModel.Actions;

public static class ActionHandlers
{

    public static string DefaultActionHandler(GameSession session, GameAction gameaction, PlayerAction action)
    {
        //By the time we get here, conditions have been met.
        //It is time to apply the effects.

        gameaction.Effects.ForEach(ef => {

            _ = session.ApplyEffect(ef, action, out var result);
        });


        return "This action is not implemented yet.";
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
            sb.AppendLine(session.CurrentScene?.Description);
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
        var player = session.Elements["player:player"];
        if (session.Elements.TryGetValue(exit, out GameElementInfo? exitElement))
        {
            if (exitElement == null)
            {
                return "Cannot find that exit";
            }

            var targetExit = exitElement.Get<Exit>();
            //Find the target scene.
            if (session.Elements.TryGetValue("scene:"+targetExit!.TargetId, out GameElementInfo? targetScene))
            {
                targetScene.IsVisible = true;
                player.Location = targetScene.Id;
            }
        }

        return HandleLook(session,gameaction,new PlayerAction(){VerbText = "Look"});

    }

  
    // public static string HandleInventoryGet(GameSession session, GameAction gameaction, PlayerAction action)
    // {
    //     StringBuilder sb = new();
    //     if (action.Targets.Count != 1)
    //     {
    //         return "You must specify a single item to get.";
    //     }
    //     var itemId = action.Targets[0];
    //     // Check if the item exists in the current scene
    //     var item = session.Elements.GetInLocation(session.CurrentScene.Id, "item")
    //         .FirstOrDefault(x => x.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
    //     if (item == null)
    //     {
    //         return $"There is no item with ID '{itemId}'.";
    //     }
    //     session.Elements[item.Id].Location = "_inventory";
    //     if (session.Elements.TryGetValue(item.Id, out var info))
    //         info.Location = "_inventory";
    //     var Item = session.GetGameElement<Item>(itemId);
    //     sb.AppendLine($"You have picked up {Item?.Name} ({Item?.Id}).");
    //     return sb.ToString();
    // }

    // public static string HandleInventoryDrop(GameSession session, GameAction gameaction,PlayerAction action)
    // { 
    //     StringBuilder sb = new();
    //     if (action.Targets.Count != 1)
    //     {
    //         return "You must specify a single item to drop.";
    //     }
    //     var itemId = action.Targets[0];
    //     // Check if the item exists in the player's inventory
    //     var item = session.Elements.GetInLocation("_inventory", "item")
    //         .FirstOrDefault(x => x.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
    //     if (item == null)
    //     {
    //         return $"You are not carrying an item with ID '{itemId}'.";
    //     }
    //     session.Elements[item.Id].Location = session.CurrentScene.Id;
    //     if (session.Elements.TryGetValue(item.Id, out var info))
    //         info.Location = session.CurrentScene.Id;
    //     var Item = session.GetGameElement<Item>(itemId);
    //     sb.AppendLine($"You have dropped {Item?.Name} ({Item?.Id}).");
    //     return sb.ToString();

    // }

   
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
