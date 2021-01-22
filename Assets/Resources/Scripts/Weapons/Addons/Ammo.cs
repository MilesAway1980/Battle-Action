using UnityEngine;
using System.Collections;

public class Ammo  {

	public int maxAmmo;
	public int startingAmmo;
	int currentAmmo;

	public Ammo() {
		currentAmmo = startingAmmo;
	}

	public Ammo(int newMaxAmmo, int newCurrentAmmo) {
		maxAmmo = newMaxAmmo;
		currentAmmo = newCurrentAmmo;
	}

	public bool UseAmmo() {
		if (currentAmmo > 0) {
			//currentAmmo--;
			return true;
		}
		return false;
	}

	public bool CheckAmmo() {
		if (currentAmmo > 0) {
			return true;
		}
		return false;
	}

	public void IncreaseAmmo(int amount) {
		if (amount > 0) {
			currentAmmo += amount;
			if (currentAmmo > maxAmmo) {
				currentAmmo = maxAmmo;
			}
		}
	}

	public void FillAmmo() {
		currentAmmo = maxAmmo;
	}
}
