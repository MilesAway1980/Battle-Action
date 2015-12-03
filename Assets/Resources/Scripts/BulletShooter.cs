using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletShooter : NetworkBehaviour {

	Ship owner;
	bool active;

	[SyncVar] 	float lastShot;

	public void setOwner(Ship newOwner) {
		owner = newOwner;
		if (owner != null) {
			active = true;
		}
	}

	void Start() {
		lastShot = Time.fixedTime;
	}

	void Update() {
		if (owner == null) {
			active = false;
		}
	}

	public bool getActive() {
		return active;
	}

	[Server]
	public void fireBullet(int whichWeapon, Vector2 startPos, float angle) {

		switch (whichWeapon) {
			//Machine gun
			case 1:				
				if (MachineGun.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}

				lastShot = Time.fixedTime;
				
				GameObject newBullet = (GameObject)Instantiate(MachineGun.getBullet());
				MachineGun mg = newBullet.GetComponent<MachineGun>();				
				mg.init(owner, startPos, angle, whichWeapon);				

				NetworkServer.Spawn(newBullet);			

				break;				
		}
	}
}
