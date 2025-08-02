namespace GameModel.Model;


public class Player : GameElement
{
    //public int HitPoints { get; set; } = 10;
    //public int ArmorClass { get; set; } = 10;

    public int HitPoints { 
        get {
            if (Attributes.TryGetValue("HitPoints", out int flag))
            {
                return flag;
            }
            return 10;
        }
        set => Attributes["HitPoints"] = value;
     }
    public int ArmorClass { 
        get {
            if (Attributes.TryGetValue("ArmorClass", out int flag))
            {
                return flag;
            }
            return 10;
        }
        set => Attributes["ArmorClass"] = value;
     }

}
