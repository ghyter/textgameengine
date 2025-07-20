// See https://aka.ms/new-console-template for more information
using GameModel;
using GameModel.Pack;

var session = GameSession.NewGame("/workspaces/textgameengine/packs/clue.json");

Console.WriteLine(session.Execute("look"));
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase)) break;
    if (input.Equals("q", StringComparison.OrdinalIgnoreCase)) break;

    var result = session.Execute(input);
    Console.WriteLine(result);
}