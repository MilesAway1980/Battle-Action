using UnityEngine;
using System.Collections;

public class BulletShooter : MonoBehaviour {

	Ship owner;

	public void setOwner(Ship newOwner) {
		owner = newOwner;
	}

	public void fireBullet(int whichWeapon, Vector2 startPos, float angle) {




		switch (whichWeapon) {
			//Machine gun
			case 1:
			//Instantiate(sphereBullet, startPos, Quaternion.identity);
				GameObject newBullet = (GameObject)Instantiate(MachineGun.getBullet(), Vector2.zero, Quaternion.identity);				
				
				MachineGun mg = newBullet.AddComponent<MachineGun>();
				mg.init(owner, startPos, angle, whichWeapon);
				break;				
		}
	}
}
