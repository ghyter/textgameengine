using GameModel.Models.Enums;

namespace GameModel.Models;

public class GamePack
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Player Player { get; set; } = new();
    public Dictionary<string, Player> Players{ get; set; } = [];
    public Dictionary<string, Scene> Scenes { get; set; } = [];
    public Dictionary<string, Item> Items { get; set; } = [];
    public Dictionary<string, Npc> Npcs { get; set; } = [];
    public Dictionary<string, Exit> Exits { get; set; } = [];

    public List<GameAction> Actions { get; set; } = [];

}

public class ExitCandidate {
    public ExitDirections Direction { get; set; }
    public Exit? Exit { get; set; }
    public Scene? TargetScene { get; set; }
}

public static class  GamePackExtensions
{
    public static List<ExitCandidate> ExitCandidates(this GamePack gamePack, Scene scene)
    {
        List<ExitCandidate> results = [];
        for (var i=0; i < 8; i++)
        {
            //Test the cardinal directions.
            int testColumn = scene.MapLocation.Column + (i switch {
                (int)ExitDirections.North => 0,
                (int)ExitDirections.NorthEast => 1,
                (int)ExitDirections.East => 1,
                (int)ExitDirections.SouthEast => 1,
                (int)ExitDirections.South => 0,
                (int)ExitDirections.SouthWest => -1,
                (int)ExitDirections.West => -1,
                (int)ExitDirections.NorthWest => -1,
                _ => 99
            });

            //Test the cardinal directions.
            int testRow = scene.MapLocation.Row + (i switch {
                (int)ExitDirections.North => -1,
                (int)ExitDirections.NorthEast => -1,
                (int)ExitDirections.East => 0,
                (int)ExitDirections.SouthEast => 1,
                (int)ExitDirections.South => 1,
                (int)ExitDirections.SouthWest => 1,
                (int)ExitDirections.West => 0,
                (int)ExitDirections.NorthWest => -1,
                _ => 99
            });

            //Test the direction to see if there is a scene in that direction
            var targetScene = gamePack.Scenes.Values.FirstOrDefault(
                s => s.MapLocation.Column == testColumn 
                && s.MapLocation.Row == testRow 
                && s.MapLocation.Level == scene.MapLocation.Level
               );
            if (targetScene != null)
            {
                //See if there is an exit in that direction already.
                var exit = scene.Exits.FirstOrDefault(e => e.Direction == (ExitDirections)i);
                results.Add( new ExitCandidate { Direction = (ExitDirections)i, Exit = exit, TargetScene = targetScene } );
            }
        }

        //Test the direction to see if there is a scene in that direction
        var testUp = gamePack.Scenes.Values.FirstOrDefault(
            s => s.MapLocation.Column == scene.MapLocation.Column
            && s.MapLocation.Row == scene.MapLocation.Row
            && s.MapLocation.Level == scene.MapLocation.Level + 1
           );
        if (testUp != null)
        {
            //See if there is an exit in that direction already.
            var exit = gamePack.Exits.Values.FirstOrDefault(e => e.Direction == ExitDirections.Up);
            results.Add(new ExitCandidate { Direction = ExitDirections.Up, Exit = exit, TargetScene = testUp });
        }

        //Test the direction to see if there is a scene in that direction
        var testDown = gamePack.Scenes.Values.FirstOrDefault(
            s => s.MapLocation.Column == scene.MapLocation.Column
            && s.MapLocation.Row == scene.MapLocation.Row
            && s.MapLocation.Level == scene.MapLocation.Level - 1
           );
        if (testDown != null)
        {
            //See if there is an exit in that direction already.
            var exit = gamePack.Exits.Values.FirstOrDefault(e => e.Direction == ExitDirections.Up);
            results.Add(new ExitCandidate { Direction = ExitDirections.Down, Exit = exit, TargetScene = testDown });
        }
        return results;
    }
}

