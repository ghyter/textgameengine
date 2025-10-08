using GameModel.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace GameModel.Models;

public class ExitTarget
{
    public required string SceneId { get; set; }
    public required string TargetId { get; set; }
    public required ExitDirections Direction { get; set; }
    public string? CustomDirection { get; set; }
    public string? CustomDirectionId { get; set; }
}


public class Exit : GameElement
{
    public override string Name
    {
        get
        {
            if (Direction == ExitDirections.Custom)
            {
                return CustomDirection;
            }
            return Direction.ToString();
        }
    }
    public List<ExitTarget> Targets { get; set; } = [];

    public string TargetId { get; set; } = string.Empty;
    public ExitDirections Direction { get; set; } = ExitDirections.Custom;
    public string CustomDirection { get; set; } = string.Empty;

}
