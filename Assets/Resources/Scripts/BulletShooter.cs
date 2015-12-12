﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BulletShooter : NetworkBehaviour {

	Ship owner;
	bool active;

	int whichWeapon;
	Vector2 startPos;
	float angle;

	float lastShot;

	bool isFiring;

	GameObject blasterObject;

	void Start() {
		isFiring = false;
		lastShot = float.MinValue;
	}

	void Update() {
		if (owner == null) { 
			active = false;
		}

		if (isFiring) {
			whichWeapon = owner.getCurrentWeapon ();
			startPos = owner.transform.position;
			angle = owner.getAngle ();
			fireBullet ();
		} else {
			if (blasterObject != null) {
				Destroy (blasterObject);
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
				if (blasterObject == null) {
					blasterObject = (GameObject)Instantiate(Blaster.getBlaster());
					NetworkServer.Spawn(blasterObject);
				}

				Blaster blaster = blasterObject.GetComponent<Blaster>();
				blaster.init(startPos, angle, owner);
				
				break;
			}

			case 5:		//Nuke
			{
				break;
			}

			case 6:		//Warp
			{
<<<<<<< HEAD
				if (Nuke.getRefireRate() > (Time.fixedTime - lastShot)) {
					return;
				}
				
				lastShot = Time.fixedTime;
				
				GameObject newBomb = (GameObject)Instantiate(Nuke.getBomb());
				Nuke nuke = newBomb.GetComponent<Nuke>();
				nuke.init(owner);
				
				NetworkServer.Spawn(newBomb);			
				
				break;	
<<<<<<< HEAD
=======
				break;
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
			}
=======
		}
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.

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
