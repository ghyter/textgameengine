using System;
using System.Text;
using System.Linq;
using GameModel;

namespace GameModel.Model;

public class Scene : GameElement
{
    public List<Exit> Exits { get; set; } = [];

    /// <summary>
    /// Returns the full description of the scene including NPCs, items and exits.
    /// </summary>
    public string ToDescription(GameSession session)
    {
        var stateId = session.Elements.TryGetValue(Id, out var info)
            ? info.State
            : DefaultState;

        StringBuilder sb = new();
        sb.AppendLine(base.ToDescription(stateId).Trim());

        var whoIsHere = session.Elements.GetInLocation(Id);

        var npcs = whoIsHere.Where(x => x.Id.StartsWith("npc:"));
        if (npcs.Any())
        {
            sb.AppendLine();
            sb.AppendLine("The following people are here:");
            int i = 0;
            foreach (var npcInfo in npcs)
            {
                var npc = session.GetGameElement<Npc>(npcInfo.Id);
                if (npc != null)
                    sb.AppendLine($"{++i}. {npc.Name} ({npc.Id})");
            }
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("No one is here.");
        }

        var items = whoIsHere.Where(x => x.Id.StartsWith("item:"));
        if (items.Any())
        {
            sb.AppendLine();
            sb.AppendLine("You see the following items:");
            int i = 0;
            foreach (var item in items)
            {
                var itm = session.GetGameElement<Item>(item.Id);
                if (itm != null)
                    sb.AppendLine($"{++i}. {itm.Name}");
            }
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("There are no items here.");
        }

        var exits = info?.Exits ?? Exits;
        var visibleExits = exits.Where(e => !string.Equals(e.DefaultState, "hidden", StringComparison.OrdinalIgnoreCase));
        if (visibleExits.Any())
        {
            sb.AppendLine();
            sb.AppendLine("You can see these exits:");
            int i = 0;
            foreach (var exit in visibleExits)
            {
                var state = exit.DefaultState ?? "open";
                var status = string.Equals(state, "locked", StringComparison.OrdinalIgnoreCase)
                    ? " (locked)"
                    : $" ({state})";
                sb.AppendLine($"{++i}. {exit.Name} ({exit.TargetId}){status}");
            }
        }
        else
        {
            sb.AppendLine();
            sb.AppendLine("There are no exits from here.");
        }

        return sb.ToString();
    }
}
