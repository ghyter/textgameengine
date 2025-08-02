using System;

namespace GameModel.Model;

public class Item : GameElement
{
    public bool IsMovable { 
        get {
            if (Flags.TryGetValue("IsMovable", out bool flag))
            {
                return flag;
            }
            return true;
        }
        set => Flags["IsMovable"] = value;
     }

}
