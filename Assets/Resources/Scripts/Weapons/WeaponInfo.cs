using UnityEngine;
using System.Collections;

public static class WeaponInfo {

	static int weaponCount = 6;

	public static string getWeaponName(int whichWeapon) {
		string weaponName = "";

		switch (whichWeapon) {
			case 1: weaponName = "Machine Gun"; break;
			case 2: weaponName = "Rockets"; break;
			case 3: weaponName = "Missile"; break;
			case 4: weaponName = "Blaster"; break;
			case 5: weaponName = "Crush"; break;
			case 6: weaponName = "Nuke"; break;
			default: weaponName = "---"; break;
		}
		return weaponName;
	}

	public static int numWeapons() {
		return weaponCount;
	}


}
