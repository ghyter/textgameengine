// See https://aka.ms/new-console-template for more information
using GameModel.Pack;
using GameModel.Session;
using static GameModel.Models.GameRound;

var session = GameSession.NewGame("/workspaces/textgameengine/packs/clue.json");
var startRound = session.Execute("look");
Console.Clear();
Console.WriteLine(startRound.Header);
Console.WriteLine(startRound.Body);
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase)) break;
    if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) break;

    var result = session.Execute(input);
    Console.Clear();
    Console.WriteLine(result.Header);
    
    if (result.Outcome == RoundOutcome.Invalid)
        Console.ForegroundColor = ConsoleColor.Red;

    Console.WriteLine(result.Body);

    if (result.Outcome == RoundOutcome.Invalid)
        Console.ResetColor(); // reset only if changed
}