using UnityEngine;
using System.Collections;
using Mirror;

public class Ammo {

	public int maxAmmo;
	[SyncVar] public int startingAmmo;
	[SyncVar] int currentAmmo;

	public Ammo() {
		
	}

	public Ammo(int newCurrentAmmo, int newMaxAmmo) {
		currentAmmo = newCurrentAmmo;
		maxAmmo = newMaxAmmo;
	}

	[Server]
	public void UseAmmo(int ammoUsed) {
		/*if (currentAmmo > 0) {
			currentAmmo--;
			return true;
		}
		return false;*/

		currentAmmo -= ammoUsed;
		if (currentAmmo < 0)
        {
			currentAmmo = 0;
        }
	}

	[Server]
	public bool CheckAmmo(int amount) {
		if (currentAmmo >= amount) {
			return true;
		}
		return false;
	}

	[Server]
	public void IncreaseAmmo(int amount) {
		if (amount > 0) {
			currentAmmo += amount;
			if (currentAmmo > maxAmmo) {
				currentAmmo = maxAmmo;
			}
		}
	}

	[Server]
	public void FillAmmo() {
		currentAmmo = maxAmmo;
	}

	public int GetCurrentAmmo()
    {
		return currentAmmo;
    }

	public int GetMaxAmmo()
    {
		return maxAmmo;
    }

	[Server]
	public void Empty()
    {
		currentAmmo = 0;
    }
}
