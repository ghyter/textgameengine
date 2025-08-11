using System;
using GameModel.Models.Enums;

namespace GameModel.Models;


public class Exit : GameElement
{
    public string TargetId { get; set; } = string.Empty;

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

    public ExitDirections Direction { get; set; } = ExitDirections.Custom;
    public string CustomDirection { get; set; } = string.Empty;
}
