using UnityEngine;
using System.Collections;

public static class WeaponInfo
{

    static readonly int weaponCount = 12;

    public static string GetWeaponName(int whichWeapon)
    {

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
    }

    public static int GetWeaponCount()
    {
        return weaponCount;
    }
}
