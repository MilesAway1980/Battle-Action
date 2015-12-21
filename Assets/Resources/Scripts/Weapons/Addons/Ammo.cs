using UnityEngine;
using System.Collections;

public class Ammo  {

	public int maxAmmo;
	public int startingAmmo;
	int currentAmmo;

	public Ammo() {
		currentAmmo = startingAmmo;
	}

	public bool useAmmo() {
		if (currentAmmo > 0) {
			currentAmmo--;
			return true;
		}
		return false;
	}

	public bool checkAmmo() {
		if (currentAmmo > 0) {
			return true;
		}
		return false;
	}

	public void incAmmo(int amount) {
		if (amount > 0) {
			currentAmmo += amount;
			if (currentAmmo > maxAmmo) {
				currentAmmo = maxAmmo;
			}
		}
	}

	public void fillAmmo() {
		currentAmmo = maxAmmo;
	}
}
