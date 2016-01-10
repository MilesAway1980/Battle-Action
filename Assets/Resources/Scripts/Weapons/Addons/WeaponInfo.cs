using UnityEngine;
using System.Collections;

public static class WeaponInfo {

	static int weaponCount = 10;

	public static string getWeaponName(int whichWeapon) {
		string weaponName = "";

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
			default: weaponName = "---"; break;
		}
		return weaponName;
	}

	public static int getWeaponCount () {
		return weaponCount;
	}
}
