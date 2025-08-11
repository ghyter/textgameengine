using System.Text;
using GameModel.Pack;
using GameModel.Actions;
using GameModel.Models;
using System.Linq;
using GameModel.Models.Constants;

namespace GameModel.Session;

public class GameSession
{
    public string GameTitle { get; set; } = "Text Game Engine";

    public ActionRegistry ActionRegistry { get; set; } = new();

    public List<GameRound> GameLog { get; set; } = [];

    private Dictionary<string, List<string>> Ordinals { get; set; } = new();

    public List<string> SceneOrdinals
    {
        get
        {
            var exists = Ordinals.TryGetValue(GameConstants.ScenePrefix, out var result);
            if (!exists)
            {
                Ordinals[GameConstants.ScenePrefix] = [];
            }
            return result ?? [];

        }
        set
        {
            Ordinals[GameConstants.ScenePrefix] = value;
        }
    }
    //public List<string> InventoryOrdinals { get; set; } = [];
    
    public List<string> InventoryOrdinals
    {
        get
        {
            var exists = Ordinals.TryGetValue(GameConstants.InventoryId, out var result);
            if (!exists)
            {
                Ordinals[GameConstants.InventoryId] = [];
            }
            return result ?? [];

        }
        set
        {
            Ordinals[GameConstants.InventoryId] = value;
        }
    }

    public GameElementState Player { get => Elements[GameConstants.PlayerId]; }

    public string Header {get
        {
            StringBuilder sb = new();
                sb.Append(GameTitle);
                sb.Append(": ");
                var sceneName = CurrentLocation?.Get<Scene>()?.Name ?? "Unknown Scene";
                sb.AppendLine(sceneName);
                //sb.AppendLine(new string('=', GameTitle.Length + sceneName.Length + 4));
            return sb.ToString();
        }
    }


    public GameElementState? CurrentLocation
    {
        get
        {
            var sceneId = Player.Location ?? string.Empty;
            return Elements.TryGetValue(sceneId, out var info) ? info : null;
        }

    }

    public GameElements Elements { get; set; } = [];

    public static GameSession NewGame(string PackPath) => GameInitializer.NewGame(PackPath);
    public static GameSession NewGame(GamePack pack) => GameInitializer.NewGame(pack);

    public GameRound Execute(string input) => GameRoundResolver.Execute(this, input);


    public IGameElement? GetGameElement(string id)
    {
        return Elements.TryGetValue(id, out var info) ? info.Element : null;
    }

    public T? GetGameElement<T>(string id) where T : class, IGameElement
    {
        return Elements.TryGetValue(id, out var info) ? info.Get<T>() : null;
    }

    public void PopulateOrdinals()
    {
        SceneOrdinals.Clear();
        SceneOrdinals.AddRange(Elements.Values
            .Where(e =>
                e.Id.StartsWith("exit:")
                && e.IsVisible
                && e.Location != null
                && e.Location.Equals(CurrentLocation?.Id)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));
        SceneOrdinals.AddRange(Elements.Values
            .Where(e =>
                e.Id.StartsWith("npc:")
                && e.IsVisible
                && e.Location != null
                && e.Location.Equals(CurrentLocation?.Id)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));
        SceneOrdinals.AddRange(Elements.Values
            .Where(e =>
                e.Id.StartsWith("item:")
                && e.IsVisible
                && e.Location != null
                && e.Location.Equals(CurrentLocation?.Id)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));


        InventoryOrdinals.Clear();
        InventoryOrdinals.AddRange(Elements.Values
            .Where(
                e => e.Location != null
                && e.IsVisible
                && e.Location.Equals(GameConstants.InventoryId)
                )
            .OrderBy(e => e.Element.Name)
            .Select(e => e.Id));

    }

    public string PrintSceneOrdinals()
    {
        StringBuilder sb = new();
        int i = 0;
        if (SceneOrdinals.Any(s => s.StartsWith("exit:")))
        {
            sb.AppendLine("Exits:");
            foreach (var exitId in SceneOrdinals.Where(s => s.StartsWith("exit:")))
            {

                var exit = Elements[exitId];
                var name = exit.Element.Name ?? exitId;
                //Since this is getting the exit from the game element, the prefix hasn't been added yet.
                var targetScene = "scene:" + GetGameElement<Exit>(exitId)?.TargetId;
                //Only show the target of the exit if the target scene is visible.
                //Whem moving through an exit the first time, the scene should be set to visible.
                var scene = Elements[targetScene!];
                if (scene?.IsVisible ?? false)
                {
                    sb.AppendLine($"{++i}. {name} ({scene.Element.Name})");
                }
                else
                {
                    sb.AppendLine($"{++i}. {name}");
                }

            }
        }
        if (SceneOrdinals.Any(s => s.StartsWith("npc:")))
        {

            sb.AppendLine("Characters:");
            foreach (var npc in SceneOrdinals.Where(s => s.StartsWith("npc:")))
            {
                var npcElement = GetGameElement<Npc>(npc);
                var name = npcElement?.Name ?? npc;
                sb.AppendLine($"{++i}. {name}");
            }
        }
        if (SceneOrdinals.Any(s => s.StartsWith("item:")))
        {
            sb.AppendLine("Items:");
            foreach (var item in SceneOrdinals.Where(s => s.StartsWith("item:")))
            {
                var itemElement = GetGameElement<Item>(item);
                var name = itemElement?.Name ?? item;
                sb.AppendLine($"{++i}. {name}");
            }
        }
        return sb.ToString();
    }

}
