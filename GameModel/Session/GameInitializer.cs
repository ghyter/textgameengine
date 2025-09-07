using System;
using GameModel.Actions;
using GameModel.Helpers;
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

        Player? player;
        if (gamePack.Players.Count == 1)
        {
            player = gamePack.Players.FirstOrDefault().Value.DeepClone();
        } else
        {
            player = new();
        }
           
        gs.Elements[GameConstants.PlayerId] = new GameElementState
        {
            Id = GameConstants.PlayerId,
            Element = player!,
            Location = player.StartingLocation,
            Attributes = player.Attributes.DeepClone(), //Load the initial state of the attributes.
            Properties = player.Properties.DeepClone(),
            Flags = player.Flags.DeepClone(),

        };


        foreach (var s in gamePack.Scenes)
        {
            var id = $"scene:{s.Key}";
            gs.Elements[id] = new GameElementState
            {
                Id = id,
                Element = s.Value.DeepClone(),
                Location = null,
                IsVisible = s.Value.IsVisible,
                State = s.Value.StartingState ?? "default"
            };
            //gs.Elements[id].Element.Id = id;
            s.Value.Exits.ForEach(exit =>
            {
                string Id = $"exit:{s.Value.Id}:{exit.TargetId}";
                gs.Elements[Id] = new()
                {
                    Id = Id,
                    IsVisible = exit.IsVisible,
                    Element = exit.DeepClone(),
                    Location = id,
                    State = exit.StartingState
                };
            });
        }

        foreach (var itm in gamePack.Items)
        {
            var id = $"item:{itm.Key}";
            gs.Elements[id] = new GameElementState
            {
                Id = id,
                Element = itm.Value.DeepClone(),
                IsVisible = itm.Value.IsVisible,
                Location = itm.Value.StartingLocation,
                State = itm.Value.StartingState ?? "default",
                Attributes = itm.Value.Attributes.DeepClone(),
                Properties = itm.Value.Properties.DeepClone(),
                Flags = itm.Value.Flags.DeepClone(),

            };
            //gs.Elements[id].Element.Id = id;
        }

        foreach (var npc in gamePack.Npcs)
        {
            var id = $"npc:{npc.Key}";
            gs.Elements[id] = new GameElementState
            {
                Id = id,
                Element = npc.Value.DeepClone(),
                IsVisible = npc.Value.IsVisible,
                Location = npc.Value.StartingLocation,
                State = npc.Value.StartingState ?? "default",
                Attributes = npc.Value.Attributes.DeepClone(),
                Properties = npc.Value.Properties.DeepClone(),
                Flags = npc.Value.Flags.DeepClone(),
            };
            //gs.Elements[id].Element.Id = id;
        }

        //Add the scene prefix for any Location element that doest start with _
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
