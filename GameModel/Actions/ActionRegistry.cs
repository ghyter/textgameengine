// using System.Text.RegularExpressions;
// using GameModel.Models;
// using GameModel.Session;

// namespace GameModel.Actions;

// public delegate string ActionHandler(GameSession session, GameAction gameaction, PlayerAction action);

// public class ActionRegistry
// {
//     private static readonly string[] ClauseSeparators = ["on", "with", "in", "into"];

//     private List<GameAction> _actions = [];
//     public IReadOnlyList<GameAction> Actions => _actions;

//     private List<PlayerAction> _history = [];
//     public List<PlayerAction> History => _history;



//     public void Register(GameAction action)
//     {
//         if (action == null) throw new ArgumentNullException(nameof(action));
//         if (string.IsNullOrWhiteSpace(action.Id)) throw new ArgumentException("Action must have a valid Id.", nameof(action));
//         _actions.Add(action);
//     }

//     public bool TryExecute(GameSession session, string input, out string result)
//     {
//         //Parse the incoming input into a PlayerAction
//         var action = Parse(input);

//         if (action == null)
//         {
//             result = $"I don't know how to '{input}'.";
//             return false;
//         }
//         //Call resolve to update the targets to ids.
//         action = ResolveTargets(session, action);

//         GameAction? gameAction = FindAction(session, action);

//         //session.ActionRegistry.History.Add(action);

//         result = gameAction?.Execute(session, action) ?? $"I don't know how to '{input}'.";

//         session.PopulateOrdinals();

//         return false;
//     }



// }

