using UnityEngine;
using System.Collections;

public static class WeaponInfo {

	static readonly int weaponCount = 13;

	public static string GetWeaponName(int whichWeapon) {
		string weaponName;

		switch (whichWeapon) {
			case 1: weaponName = "Machine Gun"; break;
			case 2: weaponName = "Rockets"; break;
			case 3: weaponName = "Missile"; break;
			case 4: weaponName = "Blaster"; break;
			case 5: weaponName = "Crush"; break;
			case 6: weaponName = "Nuke"; break;
			case 7: weaponName = "Warp"; break;
			case 8: weaponName = "Plasma"; break;
			case 9: weaponName = "Mines"; break;
			case 10: weaponName = "Decoy"; break;
			case 11: weaponName = "Turret"; break;
			case 12: weaponName = "Deactivator"; break;
			case 13: weaponName = "Deactivator Beam"; break;
			default: weaponName = "---"; break;
		}

		return weaponName;
	}

	public static int GetWeaponCount () {
		return weaponCount;
	}
}
