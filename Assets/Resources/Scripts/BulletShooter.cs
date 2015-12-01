using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletShooter : NetworkBehaviour {

	Ship owner;

	public void setOwner(Ship newOwner) {
		owner = newOwner;
	}

	[Server]
	public void fireBullet(int whichWeapon, Vector2 startPos, float angle) {




		switch (whichWeapon) {
			//Machine gun
			case 1:
			//Instantiate(sphereBullet, startPos, Quaternion.identity);
				GameObject newBullet = (GameObject)Instantiate(MachineGun.getBullet(), Vector2.zero, Quaternion.identity);				
				
				MachineGun mg = newBullet.AddComponent<MachineGun>();
				mg.init(owner, startPos, angle, whichWeapon);
				mg.setScale(0.1f);

				NetworkServer.Spawn(newBullet);
				break;				
		}
	}
}
