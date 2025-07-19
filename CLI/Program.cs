// See https://aka.ms/new-console-template for more information
using GameModel;
using GameModel.Pack;

var session = GameSession.NewGame("/workspaces/textgameengine/packs/clue.json");

while (true)
{
    Console.WriteLine(session.Screen());
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    if (input.Equals("quit", StringComparison.OrdinalIgnoreCase)) break;

    var result = session.Execute(input);
    Console.WriteLine(result);
}