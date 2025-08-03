using System;
using GameModel.Modes.Enums;
using GameModel.Models;
using GameModel.Session;

namespace GameModel.Actions;

public static class GameSessionExtensions
{

    public static void  RegisterStandardActions(this GameSession gs)
    {
        gs.ActionRegistry.Register(new GameAction
        {
            Id = "look",
            RequiredTargets = 0,
            CanonicalVerb = "look",
            VerbAliases = new() { "examine", "view", "l" },
            Handler = ActionHandlers.HandleLook
        });

        gs.ActionRegistry.Register(new GameAction
        {
            Id = "look",
            RequiredTargets = 1,
            Target1 = "*",
            CanonicalVerb = "look",
            Conditions = new()
            {
                new Condition
                {
                    GameElementId = "$Target1",
                    Rule = ConditionRuleType.InLocation,
                    Value = "$Inventory,$CurrentLocation",
                    FailMessage = "You do not see $Target1.Name here"
                }
            },
            VerbAliases = new() { "examine", "view", "l" },
            Handler = ActionHandlers.HandleLook
        });

        gs.ActionRegistry.Register(new GameAction
        {
            Id = "move",
            CanonicalVerb = "move",
            RequiredTargets = 1,
            Target1 = "exit:*",
            Conditions = new()
            {
                new Condition
                {
                    GameElementId = "$Target1",
                    Rule = ConditionRuleType.InLocation,
                    Value = "$CurrentLocation",
                    FailMessage = "$Target1.Name is not here."
                },
                new Condition
                {
                    GameElementId = "$Target1",
                    Rule = ConditionRuleType.StateValue,
                    Value = "open",
                    FailMessage = "$Target1.Name is $Target1.State."
                }
            },
            VerbAliases = new() { "go", "m", "g" },
            Handler = ActionHandlers.HandleMove
            

        });

        // gs.ActionRegistry.Register(new GameAction
        // {
        //     Id = "history",
        //     CanonicalVerb = "history",
        //     VerbAliases = new() { "hist" },
        //     Handler = ActionHandlers.HandleHistory
        // });
        gs.ActionRegistry.Register(new GameAction
        {
            Id = "debug",
            CanonicalVerb = "debug",
            VerbAliases = new() { "log" },
            Handler = ActionHandlers.HandleDebug
        });

        gs.ActionRegistry.Register(new GameAction
        {
            Id = "inventory",
            CanonicalVerb = "inventory",
            VerbAliases = new() { "inv", "i" },
            Handler = ActionHandlers.HandleInventory
        });

        

        gs.ActionRegistry.Register(new GameAction
        {
            Id = "get",
            RequiredTargets = 1,
            CanonicalVerb = "get",
            Target1 = "item:*",
            VerbAliases = new() { "grab", "g", "pick up" },
            Conditions = [
                new Condition
                {
                    GameElementId = "$Target1",
                    Rule = ConditionRuleType.InLocation,
                    Value = "$CurrentLocation",
                    FailMessage = "$Target1.Name is not here"
                },
                new Condition
                {
                    GameElementId = "$Target1",
                    Rule = ConditionRuleType.IsMovable,
                    FailMessage = "You cannot move $Target1.Name"
                }

            ],
            Effects = [
                new(){
                    GameElementId = "$Target1",
                    Type= EffectType.ChangeLocation,
                    NewValue = "$Inventory",
                    SuccessMessage = "$Target1.Name: Taken"
                }
            ]
        });

        gs.ActionRegistry.Register(new GameAction
        {
            Id = "drop",
            RequiredTargets = 1,
            CanonicalVerb = "drop",
            Target1 = "item:*",
            VerbAliases = new() { "put down", "d"},
            Conditions = [
                new Condition
                {
                    GameElementId = "$Target1",
                    Rule = ConditionRuleType.InLocation,
                    Value = "$Inventory",
                    FailMessage = "$Target1.Name is not in inventory"
                }
            ],
            Effects = [
                new(){
                    GameElementId = "$Target1",
                    Type= EffectType.ChangeLocation,
                    NewValue = "$CurrentLocation",
                    SuccessMessage = "$Target1.Name: Dropped"
                }
            ]
        });
    }
}
