using System;
using GameModel.Models.Enums;

namespace GameModel.Models;


public class Exit : GameElement
{
    public string TargetId { get; set; } = string.Empty;

    public ExitDirections Direction { get; set; } = ExitDirections.Custom;
    public string CustomDirection { get; set; } = string.Empty;
}
