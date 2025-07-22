using System;
using System.Text;
using GameModel.Model;

namespace GameModel.Actions;

public static class ActionHandlers
{
    public static string HandleLook(GameSession session, PlayerAction action)
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
                return $"There is no such thing as {targetId}.";
            }
            return $"{target.Description} ({target.Id})";

        }
        else
        {
            return session.CurrentScene.ToDescription(session);
        }

    }

    public static string HandleMove(GameSession session, PlayerAction action)
    {
        if (action.Targets.Count != 1)
            return "You must specify a single exit to move through.";

        var input = action.Targets[0];
        var exits = session.CurrentScene.Exits;

        Exit? exitId = null;

        if (int.TryParse(input, out var index))
        {
            // Convert from 1-based index to 0-based array position
            if (index >= 1 && index <= exits.Count)
            {
                exitId = exits[index - 1];
                action.Targets[0] = exitId.Id; // Update the action target to the actual exit ID
            }

        }
        else if (exits.Exists(x => x.Id == input))
        {
            exitId = exits.Where(x => x.Id == input).First();
        }

        if (exitId == null)
            return $"There is no exit '{input}' from here.";

        session.CurrentScene = session.GetGameElement<Scene>(exitId.TargetId)!;
        session.Elements["player:player"].LocationId = session.CurrentScene.Id; 

        return session.CurrentScene.ToDescription(session);
    }



    public static string HandleInventoryGet(GameSession session, PlayerAction action)
    {
        StringBuilder sb = new();
        if (action.Targets.Count != 1)
        {
            return "You must specify a single item to get.";
        }
        var itemId = action.Targets[0];
        // Check if the item exists in the current scene
        var item = session.Elements.GetInLocation(session.CurrentScene.Id, "item")
            .FirstOrDefault(x => x.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            return $"There is no item with ID '{itemId}'.";
        }
        session.Elements[item.Id].LocationId = "_inventory";
        if (session.Elements.TryGetValue(item.Id, out var info))
            info.LocationId = "_inventory";
        var Item = session.GetGameElement<Item>(itemId);
        sb.AppendLine($"You have picked up {Item?.Name} ({Item?.Id}).");
        return sb.ToString();
    }

    public static string HandleInventoryDrop(GameSession session, PlayerAction action)
    { 
        StringBuilder sb = new();
        if (action.Targets.Count != 1)
        {
            return "You must specify a single item to drop.";
        }
        var itemId = action.Targets[0];
        // Check if the item exists in the player's inventory
        var item = session.Elements.GetInLocation("_inventory", "item")
            .FirstOrDefault(x => x.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase));
        if (item == null)
        {
            return $"You are not carrying an item with ID '{itemId}'.";
        }
        session.Elements[item.Id].LocationId = session.CurrentScene.Id;
        if (session.Elements.TryGetValue(item.Id, out var info))
            info.LocationId = session.CurrentScene.Id;
        var Item = session.GetGameElement<Item>(itemId);
        sb.AppendLine($"You have dropped {Item?.Name} ({Item?.Id}).");
        return sb.ToString();

    }

    public static string HandleInventory(GameSession session, PlayerAction action)
    {
        StringBuilder sb = new();
        var inventory = session.Elements.GetInLocation("_inventory", "item");
        if (!inventory.Any())
        {
            sb.AppendLine("You are not carrying anything.");
        }
        else
        {
            sb.AppendLine("You are carrying:");
            foreach (var item in inventory)
            {
                var Item = session.GetGameElement<Item>(item.Id);
                if (Item != null)
                    sb.AppendLine($"{Item.Name} ({Item.Id})");
            }
        }
        return sb.ToString();
    }

    public static string HandleHistory(GameSession session, PlayerAction action)
    {
        StringBuilder sb = new();
        sb.AppendLine("Action History:");
        if (session.ActionHistory.Count == 0)
        {
            sb.AppendLine("No actions have been taken yet.");
        }
        else
        {
            for (int i = 0; i < session.ActionHistory.Count; i++)
            {
                var act = session.ActionHistory[i];
                sb.AppendLine($"{i + 1}: {act.VerbText} {string.Join(" ", act.Targets)}");
            }
        }
        return sb.ToString();

    }


}
