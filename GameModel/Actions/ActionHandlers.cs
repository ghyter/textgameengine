using System;
using System.Text;
using GameModel.Model;

namespace GameModel.Actions;

public static class ActionHandlers
{
    public static string HandleLook(GameSession session, GameAction gameaction, PlayerAction action)
    {
        if (action.Targets.Any())
        {
            if (action.Targets.Count != 1)
            {
                return "You can only look at one thing at a time.";
            }

            var targetId = action.Targets[0];
            var target = session.GetGameElement(targetId);
            if (target == null)
            {
                return $"I do not see a '{targetId}'.";
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


    public static string HandleDataAction(GameSession session, GameAction gameaction, PlayerAction action)
    { 
        return "This action is not implemented yet.";
    }

    // public static string HandleMove(GameSession session, GameAction gameaction,PlayerAction action)
    // {
    //     if (action.Targets.Count != 1)
    //         return "You must specify a single exit to move through.";

    //     var input = action.Targets[0];
    //     var exits = session.CurrentScene.Exits;

    //     Exit? exitId = null;

    //     if (int.TryParse(input, out var index))
    //     {
    //         // Convert from 1-based index to 0-based array position
    //         if (index >= 1 && index <= exits.Count)
    //         {
    //             exitId = exits[index - 1];
    //             action.Targets[0] = exitId.Id; // Update the action target to the actual exit ID
    //         }

    //     }
    //     else if (exits.Exists(x => x.Id == input))
    //     {
    //         exitId = exits.Where(x => x.Id == input).First();
    //     }

    //     if (exitId == null)
    //         return $"There is no exit '{input}' from here.";

    //     session.CurrentScene = session.GetGameElement<Scene>(exitId.TargetId)!;
    //     session.Elements["player:player"].Location = session.CurrentScene.Id; 

    //     return LookScene(session, gameaction, action, session.CurrentScene);
    // }



    // private static string LookScene(GameSession session, GameAction gameaction, PlayerAction action, Scene scene)
    // {
    //     StringBuilder sb = new();
    //     int i = 1;
    //     //I am looking at the scene
    //     sb.AppendLine(scene.Description);
    //     var whoishere = session.Elements.GetInLocation(scene.Id);

    //     var npcs = whoishere.Where(x => x.Id.StartsWith("npc:"));
    //     if (npcs.Any())
    //     {
    //         sb.AppendLine("");
    //         sb.AppendLine("The following people are here:");
    //         i = 0;
    //         foreach (var item in npcs)
    //         {
    //             var npc = session.GetGameElement<Npc>(item.Id);
    //             if (npc != null)
    //                 sb.AppendLine($"{++i}. {npc.Name} ({npc.Id})");
    //         }
    //     }
    //     else
    //     {
    //         sb.AppendLine("");
    //         sb.AppendLine("No one is here.");
    //     }

    //     var items = whoishere.Where(x => x.Id.StartsWith("item:"));
    //     if (items.Any())
    //     {
    //         sb.AppendLine("");
    //         sb.AppendLine("You see the following items:");
    //         i = 0;
    //         foreach (var item in items)
    //         {
    //             var itm = session.GetGameElement<Item>(item.Id);
    //             if (itm != null)
    //                 sb.AppendLine($"{++i}. {itm.Name}");
    //         }
    //     }
    //     else
    //     {
    //         sb.AppendLine("");
    //         sb.AppendLine("There are no items here.");
    //     }


    //     if (scene.Exits.ToList().Any())
    //     {
    //         sb.AppendLine("");
    //         sb.AppendLine("You can see these exits:");
    //         i = 0;
    //         foreach (var exit in scene.Exits)
    //         {
    //             var exScene = session.GetGameElement<Scene>(exit.TargetId);
    //             var name = exScene?.Name ?? exit.Name;
    //             sb.AppendLine($"{++i}. {exit.Name} ({exit.TargetId})");
    //         }
    //     }
    //     else
    //     {
    //         sb.AppendLine("");
    //         sb.AppendLine("There are no exits from here.");
    //     }

    //     return sb.ToString();
    // }

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

    // public static string HandleInventory(GameSession session, GameAction gameaction, PlayerAction action)
    // {
    //     StringBuilder sb = new();
    //     var inventory = session.Elements.GetInLocation("_inventory", "item");
    //     if (!inventory.Any())
    //     {
    //         sb.AppendLine("You are not carrying anything.");
    //     }
    //     else
    //     {
    //         sb.AppendLine("You are carrying:");
    //         foreach (var item in inventory)
    //         {
    //             var Item = session.GetGameElement<Item>(item.Id);
    //             if (Item != null)
    //                 sb.AppendLine($"{Item.Name} ({Item.Id})");
    //         }
    //     }
    //     return sb.ToString();
    // }

    // public static string HandleHistory(GameSession session, GameAction gameaction, PlayerAction action)
    // {
    //     StringBuilder sb = new();
    //     sb.AppendLine("Action History:");

    //     if (session.ActionRegistry.History.Count == 0)
    //     {
    //         sb.AppendLine("No actions have been taken yet.");
    //     }
    //     else
    //     {
    //         for (int i = 0; i < session.ActionRegistry.History.Count; i++)
    //         {
    //             var act = session.ActionRegistry.History[i];
    //             sb.AppendLine($"{i + 1}: {act.VerbText} {string.Join(" ", act.Targets)}");
    //         }
    //     }
    //     return sb.ToString();

    // }
}
