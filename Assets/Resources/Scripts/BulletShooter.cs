using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BulletShooter : NetworkBehaviour {

	Ship owner;
	bool active;

	int whichWeapon;
	Vector2 startPos;
	float angle;

	float lastShot;

	bool isFiring;
	bool firstShot;
	float shootTimer;

	GameObject blasterObject;
	GameObject warper;

	ObjectList chargingPlasmas;

	void Start() {
		isFiring = false;
		lastShot = float.MinValue;
		firstShot = true;
		shootTimer = 0;
	}

	void FixedUpdate() {
		if (owner == null) { 
			active = false;
		}

		if (isFiring) {
			whichWeapon = owner.getCurrentWeapon ();
			startPos = owner.transform.position;
			angle = owner.getAngle ();
			fireBullet ();
			shootTimer += Time.deltaTime;
		} else {
			firstShot = true;
			if (shootTimer != 0) {
				release ();	
				shootTimer = 0;
			}
		}
	}

	public void setIsFiring(bool newIsFiring) {
		isFiring = newIsFiring;
	}

	public void setOwner(Ship newOwner) {
		owner = newOwner;
		if (owner != null) {
			active = true;
		}
	}

	public bool getActive() {
		return active;
	}

	void release() {
		switch (whichWeapon) {
			case 4: //Blaster
			{
				if (blasterObject != null) {
					Destroy (blasterObject);
				}
				break;
			}
			case 8:	//Plasma
			{
				if (chargingPlasmas != null) {
					GameObject[] plasmas = chargingPlasmas.getObjects ();
					for (int i = 0; i < plasmas.Length; i++) {
						Plasma plasma = plasmas [i].GetComponent<Plasma> ();
						plasma.release ();
					}
				}
				break;
			}
		}
	}

	public void fireBullet() {

		switch (whichWeapon) {

			case 1:		//Machine gun
			{
				if (MachineGun.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}

				lastShot = Time.fixedTime;
				
				GameObject newBullet = (GameObject)Instantiate(MachineGun.getBullet());
				MachineGun mg = newBullet.GetComponent<MachineGun>();				
				mg.init(owner, startPos, angle);				

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
					rocket.init(owner, startPos, newAngle);					
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
					missile.init(owner, startPos, newAngle);					
					NetworkServer.Spawn(newBullet);						
				}

				break;
			}

			case 4:		//Blaster
			{
				if (blasterObject == null) {
					blasterObject = (GameObject)Instantiate(Blaster.getBlaster());
					NetworkServer.Spawn(blasterObject);
				}

				Blaster blaster = blasterObject.GetComponent<Blaster>();
				blaster.init(startPos, angle, owner);
				
				break;
			}

			case 5:		//Crush / Flak
			{
				if (Crush.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}
				
				lastShot = Time.fixedTime;
				
				float newAngle = angle;
				GameObject newBullet = (GameObject)Instantiate(Crush.getBullet());
				Crush crush = newBullet.GetComponent<Crush>();
				crush.init(owner, startPos, newAngle);					
				NetworkServer.Spawn(newBullet);						

				break;
			}

			case 6:		//Nuke
			{
				if (Nuke.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}
				
				lastShot = Time.fixedTime;
				
				GameObject newBomb = (GameObject)Instantiate(Nuke.getBomb());
				Nuke nuke = newBomb.GetComponent<Nuke>();
				nuke.init(owner);
				
				NetworkServer.Spawn(newBomb);
				
				break;	
			}

			case 7:		//Warp
			{
				if (warper == null) {
					warper = new GameObject();
					warper.name = "Warp";
					warper.transform.parent = transform;
					warper.AddComponent<Warp>();
					warper.GetComponent<Warp>().setOwner(owner);
				}
				break;
			}

			case 8:		//Plasma
			{
				if (Plasma.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}

				float newAngle = 0;
				float angleChange = 0;
				float arc = 0;

				if (firstShot == false) {
					//Increase charge

					if (chargingPlasmas != null) {
						GameObject[] plasmas = chargingPlasmas.getObjects ();

						for (int i = 0; i < plasmas.Length; i++) {
							Plasma plasma = plasmas [i].GetComponent<Plasma> ();
							plasma.incCharge (Time.deltaTime);

							if (i == 0) {
								arc = plasma.getArc ();
								newAngle = angle - (arc / 2.0f);
								if (Plasma.getBulletsPerShot () > 1) {
									angleChange = (arc / (Plasma.getBulletsPerShot () - 1));
								} else {
									angleChange = 0;
									newAngle = angle;
								}
							}

							plasma.setAngle (newAngle);

							newAngle += angleChange;
						}
					}

					return;
				}

				firstShot = false;
				lastShot = Time.fixedTime;

				if (chargingPlasmas == null) {
					chargingPlasmas = new ObjectList ();
				}

				chargingPlasmas.clearList ();

				for (int i = 0; i < Plasma.getBulletsPerShot(); i++) {

					GameObject newBullet = (GameObject)Instantiate (Plasma.getBullet ());
					Plasma plasma = newBullet.GetComponent<Plasma> ();
					plasma.setRadius (0);

					chargingPlasmas.addObject (newBullet);

					if (i == 0) {
						arc = plasma.getArc ();
						newAngle = angle - (arc / 2.0f);
						if (Plasma.getBulletsPerShot () > 1) {
							angleChange = (arc / (Plasma.getBulletsPerShot () - 1));
						} else {
							angleChange = 0;
							newAngle = angle;
						}
					}

					plasma.init(owner, startPos, newAngle);					
					NetworkServer.Spawn(newBullet);

					newAngle += angleChange;
				}
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
