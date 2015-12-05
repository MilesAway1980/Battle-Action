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
		lastShot = float.MinValue;
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

			case 1:		//Machine gun
			{
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

			case 2:		//Rockets
			{
				
				if (Rocket.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}
				
				lastShot = Time.fixedTime;
				
				float newAngle = angle;
				for (int i = 0; i < Rocket.getBulletsPerShot(); i++) {
					newAngle += (360.0f / (float)Rocket.getBulletsPerShot());
					GameObject newBullet = (GameObject)Instantiate(Rocket.getBullet());
					Rocket rocket = newBullet.GetComponent<Rocket>();				
					rocket.init(owner, startPos, newAngle, whichWeapon);					
					NetworkServer.Spawn(newBullet);						
				}
				break;
			}

			case 3:		//Missile
			{
				if (Missile.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}
				
				lastShot = Time.fixedTime;
				
				float newAngle = angle;
				for (int i = 0; i < Missile.getBulletsPerShot(); i++) {
					newAngle += (360.0f / (float)Missile.getBulletsPerShot());
					GameObject newBullet = (GameObject)Instantiate(Missile.getBullet());
					Missile missile = newBullet.GetComponent<Missile>();				
					missile.init(owner, startPos, newAngle, whichWeapon);					
					NetworkServer.Spawn(newBullet);						
				}

				break;
			}

			case 4:		//Blaster
			{
				break;
			}

			case 5:		//Nuke
			{
				break;
			}

			case 6:		//Warp
			{
				break;
			}

			case 7:		//Flak / Crush
			{
				break;
			}

			case 8:		//Plasma
			{
				break;
			}

			case 9:		//Turrets
			{
				break;
			}

			case 10:	//Mines
			{
				break;
			}

			case 11:	//Deactivator
			{
				break;
			}

			case 12:	//Decoy
			{
				break;
			}
		}
	}
}
