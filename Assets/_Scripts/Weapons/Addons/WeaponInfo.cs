using UnityEngine;
using System.Collections;

public static class WeaponInfo
{

    static readonly int weaponCount = 12;

    public static string GetWeaponName(int whichWeapon)
    {
        /*
        return whichWeapon switch
        {
            0 => "Machine Gun",
            1 => "Rockets",
            2 => "Missile",
            3 => "Blaster",
            4 => "Crush",
            5 => "Nuke",
            6 => "Warp",
            7 => "Plasma",
            8 => "Mines",
            9 => "Decoy",
            10 => "Turret",
            11 => "Deactivator",
            //case 13: return "Deactivator Beam"; 
            _ => "---",
        };
        */

        switch (whichWeapon)
        {
            case 0: return "Machine Gun";
            case 1: return "Rockets";
            case 2: return "Missile";
            case 3: return "Blaster";
            case 4: return "Crush";
            case 5: return "Nuke";
            case 6: return "Warp";
            case 7: return "Plasma";
            case 8: return "Mines";
            case 9: return "Decoy";
            case 10: return "Turret";
            case 11: return "Deactivator";
            default: return "---";
        }
    }

    public static int GetWeaponCount()
    {
        return weaponCount;
    }
}
