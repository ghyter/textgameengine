using System.Text;

namespace GameModel.Models;

public class MapLocation 
{
    public int Row { get; set; }
    public int Column { get; set; }
    public int Height { get; set; } = 1;
    public int Width { get; set; } = 1;
    public int Level { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"({Row},{Column},{Level})  {Height} x {Width}");
        return sb.ToString();
    }
}