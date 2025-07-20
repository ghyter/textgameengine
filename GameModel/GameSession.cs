using System.Text;
using GameModel.Pack;
using GameModel.Actions;
using GameModel.Model;

namespace GameModel;

public class GameSession
{
    private GameActionRegistry _actionRegistry { get; set; } = new();

    public GamePack GamePack { get; set; } = default!;
    public Player Player { get; set; } = new();
    public List<PlayerAction> ActionHistory { get; set; } = [];

    public SceneMap SceneMap { get; set; } = new();
    public Scene CurrentScene { get; set; } = new();

    public Dictionary<string, string> TypeLookup { get; set; } = [];
    public Dictionary<string, string> StateLookup { get; set; } = [];

    public static GameSession NewGame(string PackPath)
    {
        var gamePack = GamePackLoader.Load(PackPath);

        GameSession gs = new();
        gs.GamePack = gamePack;
        gs.SceneMap = gamePack.InitialSceneMap;


        string playerlocation = gs.SceneMap.GetLocationOf("player", "player") ?? "default";
        gs.CurrentScene = gs.GamePack.Scenes[playerlocation];

        foreach (var s in gs.GamePack.Scenes)
        {
            gs.TypeLookup[s.Key] = "scene";
            gs.StateLookup[s.Key] = "default";
        }
        foreach (var i in gs.GamePack.Items)
        {
            gs.TypeLookup[i.Key] = "item";
            gs.StateLookup[i.Key] = "default";
        }
        foreach (var npc in gs.GamePack.Npcs)
        {
            gs.TypeLookup[npc.Key] = "npc";
            gs.StateLookup[npc.Key] = "default";
        }


        gs._actionRegistry.Register(Handlers.HandleLook, "look", "examine", "view","l");
        gs._actionRegistry.Register(Handlers.HandleMove, "move", "go","m", "g");
        gs._actionRegistry.Register(Handlers.HandleHistory, "history", "hist");
        gs._actionRegistry.Register(Handlers.HandleInventory, "inventory", "inv", "i");
        gs._actionRegistry.Register(Handlers.HandleInventoryGet, "get", "grab", "g");
        gs._actionRegistry.Register(Handlers.HandleInventoryDrop, "drop", "d");q


        return gs;
    }

    public IGameElement? GetGameElement(string id)
    {
        var type = TypeLookup.GetValueOrDefault(id, "unknown");
        return type switch
        {
            "scene" => GamePack.Scenes[id],
            "item" => GamePack.Items[id],
            "npc" => GamePack.Npcs[id],
            _ => null
        };
    }

public T? GetGameElement<T>(string id) where T : class
{
    var type = TypeLookup.GetValueOrDefault(id, "unknown");

    if (typeof(T) == typeof(Scene) && type == "scene")
        return GamePack.Scenes.TryGetValue(id, out var scene) ? scene as T : null;

    if (typeof(T) == typeof(GameItem) && type == "item")
        return GamePack.Items.TryGetValue(id, out var item) ? item as T : null;

    if (typeof(T) == typeof(Npc) && type == "npc")
        return GamePack.Npcs.TryGetValue(id, out var npc) ? npc as T : null;

    return null;
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
