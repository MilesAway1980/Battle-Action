using UnityEngine;
using System.Collections;

public static class WeaponText {



	public static string getWeaponName(int whichWeapon) {
		string weaponName = "";

		switch (whichWeapon) {
		case 1: weaponName = "Machine Gun"; break;
		case 2: weaponName = "Rockets"; break;
		case 3: weaponName = "Missile"; break;
		case 4: weaponName = "Beacon"; break;
		}
		return weaponName;
	}


}
