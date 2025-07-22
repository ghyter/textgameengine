using System.Text;
using GameModel.Pack;
using GameModel.Actions;
using GameModel.Model;
using System.Linq;

namespace GameModel;

public class GameSession
{
    const string _playerid = "player:player";
    
    public string GameTitle { get; set; } = "Text Game Engine";

    private ActionRegistry _actionRegistry { get; set; } = new();
       
    public List<PlayerAction> ActionHistory { get; set; } = [];

    public Scene CurrentScene { get; set; } = new();

    public GameElements Elements { get; set; } = [];

    public static GameSession NewGame(string PackPath)
    {
        var _gamePack = GamePackLoader.Load(PackPath);
        if (_gamePack == null)
        {
            throw new ArgumentException("Invalid game pack path or format.");
        }
        GameSession gs = new();
        
        gs.GameTitle = _gamePack.Title ?? "Text Game Engine";
        gs.Elements[_playerid] = new GameElementInfo
        {
            
            Id = _playerid,
            Element = _gamePack.Player,
            Location = "scene:" + _gamePack.Player.StartingLocation
        };
       


        foreach (var s in _gamePack.Scenes)
        {
            var id = $"scene:{s.Key}";
            gs.Elements[id] = new GameElementInfo
            {
                Id = id,
                Element = s.Value,
                Location = null,
                Exits = s.Value.Exits.ToList(),
                State = s.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
            gs.Elements[id].Exits.ForEach(exit => 
            {
                exit.Id = $"exit:{s.Value.Id}:{exit.TargetId}";
            });
        }

        foreach (var i in _gamePack.Items)
        {
            var id = $"item:{i.Key}";
            gs.Elements[id] = new GameElementInfo
            {
                Id = id,
                Element = i.Value,
                Location = "scene:" + i.Value.StartingLocation,
                State = i.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
        }

        foreach (var npc in _gamePack.Npcs)
        {
            var id = $"npc:{npc.Key}";
            gs.Elements[id] = new GameElementInfo
            {
                Id = id,
                Element = npc.Value,
                Location = "scene:" + npc.Value.StartingLocation,
                State = npc.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
        }

        gs.CurrentScene = gs.Elements[gs.Elements[_playerid].Location!].Get<Scene>()!;
        gs._actionRegistry.Register(ActionHandlers.HandleLook, "look", "examine", "view","l");
        gs._actionRegistry.Register(ActionHandlers.HandleMove, "move", "go","m", "g");
        gs._actionRegistry.Register(ActionHandlers.HandleHistory, "history", "hist");
        gs._actionRegistry.Register(ActionHandlers.HandleInventory, "inventory", "inv", "i");
        gs._actionRegistry.Register(ActionHandlers.HandleInventoryGet, "get", "grab", "g");
        gs._actionRegistry.Register(ActionHandlers.HandleInventoryDrop, "drop", "d");

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
        var actionresult = _actionRegistry.TryExecute(this, input, out var result) ? result : result;
        
        //Header
        
        sb.Append(GameTitle);
        sb.Append(": ");
        sb.AppendLine(CurrentScene.Name);
        sb.AppendLine(new string('=', GameTitle.Length + CurrentScene.Name.Length + 2));
        sb.AppendLine(actionresult);

        return sb.ToString();
    }



}
