using System.Text;
using GameModel.Pack;
using GameModel.Actions;
using GameModel.Model;
using System.Linq;

namespace GameModel;

public class GameSession
{
    private GameActionRegistry _actionRegistry { get; set; } = new();

    public GamePack GamePack { get; set; } = default!;
    
    public List<PlayerAction> ActionHistory { get; set; } = [];

    public Scene CurrentScene { get; set; } = new();

    public GameElements Elements { get; set; } = [];

    public static GameSession NewGame(string PackPath)
    {
        var gamePack = GamePackLoader.Load(PackPath);

        GameSession gs = new();
        gs.GamePack = gamePack;

        gs.Elements["player"] = new GameElementInfo
        {
            Id = "player",
            Type = "player",
            Element = new Player()
            {
                Id = "player",
                Name = "Player",
                Description = "You are the player character."
            },
            LocationId = gamePack.InitialSceneMap.GetLocationOf("player", "player")
        };
        gs.CurrentScene = gs.GamePack.Scenes[gs.Elements["player"].LocationId ?? "default"];


        foreach (var s in gs.GamePack.Scenes)
        {
            gs.Elements[s.Key] = new GameElementInfo
            {
                Id = s.Key,
                Type = "scene",
                Element = s.Value,
                LocationId = null,
                Exits = s.Value.Exits.ToList()
            };
        }

        foreach (var i in gs.GamePack.Items)
        {
            gs.Elements[i.Key] = new GameElementInfo
            {
                Id = i.Key,
                Type = "item",
                Element = i.Value,
                LocationId = gs.GamePack.InitialSceneMap.GetLocationOf("item", i.Key)
            };
        }

        foreach (var npc in gs.GamePack.Npcs)
        {
            gs.Elements[npc.Key] = new GameElementInfo
            {
                Id = npc.Key,
                Type = "npc",
                Element = npc.Value,
                LocationId = gs.GamePack.InitialSceneMap.GetLocationOf("npc", npc.Key)
            };
        }


        gs._actionRegistry.Register(Handlers.HandleLook, "look", "examine", "view","l");
        gs._actionRegistry.Register(Handlers.HandleMove, "move", "go","m", "g");
        gs._actionRegistry.Register(Handlers.HandleHistory, "history", "hist");
        gs._actionRegistry.Register(Handlers.HandleInventory, "inventory", "inv", "i");
        gs._actionRegistry.Register(Handlers.HandleInventoryGet, "get", "grab", "g");
        gs._actionRegistry.Register(Handlers.HandleInventoryDrop, "drop", "d");

        return gs;
    }

    public IGameElement? GetGameElement(string id)
    {
        return Elements.TryGetValue(id, out var info) ? info.Element : null;
    }

    public T? GetGameElement<T>(string id) where T : class, IGameElement
    {
        return Elements.TryGetValue(id, out var info) ? info.Get<T>() : null;
    }


    public string Execute(string input)
    {
        StringBuilder sb = new();
        var action = PlayerAction.Parse(input);
     
        var actionresult = _actionRegistry.TryExecute(this, action, out var result) ? result : result;
        ActionHistory.Add(action);

        //Header
        sb.Append($"{ActionHistory.Count} ");
        sb.Append(GamePack.Title);
        sb.Append(": ");
        sb.AppendLine(CurrentScene.Name);
        sb.AppendLine(new string('=', GamePack.Title.Length + CurrentScene.Name.Length + 2));
        sb.AppendLine(actionresult);

        return sb.ToString();
    }



}
