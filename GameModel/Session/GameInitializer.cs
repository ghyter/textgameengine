using System;
using GameModel.Actions;
using GameModel.Models;
using GameModel.Models.Constants;
using GameModel.Pack;

namespace GameModel.Session;

public class GameInitializer
{
    public static GameSession NewGame(string PackPath)
    {
        var _gamePack = GamePackLoader.Load(PackPath);
        if (_gamePack == null)
        {
            throw new ArgumentException("Invalid game pack path or format.");
        }
        return NewGame(_gamePack);
    }

    public static GameSession NewGame(GamePack pack)
    {
        GameSession gs = new();

        LoadGamePack(gs, pack);
        gs.RegisterStandardActions();

        return gs;
    }

    private static void LoadGamePack(GameSession gs, GamePack gamePack)
    {
        gs.GameTitle = gamePack.Title ?? "Text Game Engine";
        gs.Elements[GameConstants.PlayerId] = new GameElementState
        {

            Id = GameConstants.PlayerId,
            Element = gamePack.Player,
            Location = gamePack.Player.StartingLocation,
            Attributes = gamePack.Player.Attributes, //Load the initial state of the attributes.

        };

        foreach (var s in gamePack.Scenes)
        {
            var id = $"scene:{s.Key}";
            gs.Elements[id] = new GameElementState
            {
                Id = id,
                Element = s.Value,
                Location = null,
                IsVisible = s.Value.IsVisible,
                State = s.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
            s.Value.Exits.ForEach(exit =>
            {
                exit.Id = $"exit:{s.Value.Id}:{exit.TargetId}";
                gs.Elements[exit.Id] = new()
                {
                    Id = exit.Id,
                    IsVisible = exit.IsVisible,
                    Element = exit,
                    Location = id,
                    State = exit.StartingState
                };
            });
        }

        foreach (var i in gamePack.Items)
        {
            var id = $"item:{i.Key}";
            gs.Elements[id] = new GameElementState
            {
                Id = id,
                Element = i.Value,
                IsVisible = i.Value.IsVisible,
                Location = i.Value.StartingLocation,
                State = i.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
        }

        foreach (var npc in gamePack.Npcs)
        {
            var id = $"npc:{npc.Key}";
            gs.Elements[id] = new GameElementState
            {
                Id = id,
                Element = npc.Value,
                IsVisible = npc.Value.IsVisible,
                Location = npc.Value.StartingLocation,
                State = npc.Value.StartingState ?? "default"
            };
            gs.Elements[id].Element.Id = id;
        }

        //Add the scene prefix for any element that doest start with _
        foreach (var element in gs.Elements.Values)
        {
            if (element.Location != null
                && !element.Location.StartsWith("_")
                && !element.Location.StartsWith("scene:")
              )
            {
                element.Location = $"scene:{element.Location}";
            }
        }

        //Add the data driven actions.
        foreach (var action in gamePack.Actions)
        {
            gs.ActionRegistry.Register(action);
        }

    }

}
