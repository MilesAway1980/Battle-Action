using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BulletShooter : NetworkBehaviour {

	/*Player owner;					//The Player that owns the bullet shooter
	Ship ownerShip;					//The Player's ship that is shooting*/

	GameObject owner;

	[SyncVar] int currentWeapon;				//Which weapon the ship is equipped with
	Vector2 startPos;				//The weapon's starting position
	float angle;					//The weapon's starting angle

	bool isFiring;					//Used to keep track if the bullet shooter is firing/charging, or should be reset
	bool firstShot;					//Keeps track if the shot is the very first shot of a rapid fire
	float shootTimer;				//How long the shooter has been firing in rapid fire

	GameObject blasterObject;		//The object used to fire the Blaster laser
	ObjectList chargingPlasmas;		//A list used to contain all of the ship's Plasma balls

	ShotTimer[] shotTimer;			//The ShotTimers for each weapon
	Ammo[] ammo;

	float prevShot;

	void Start() {
		isFiring = false;
		firstShot = true;
		shootTimer = 0;
		shotTimer = new ShotTimer [WeaponInfo.getWeaponCount () + 1];
		ammo = new Ammo[WeaponInfo.getWeaponCount () + 1];
	}

	void FixedUpdate() {

		if (owner == null) { 
			return;
		}

		/*if (ownerShip == null) {			//If the player has no ship, it was recently destroyed.  
			ownerShip = owner.getShip();	//Get their new ship
			if (ownerShip == null) {		//If they still don't have one, it hasn't respawned
				return;						//Exit and check later
			}
		}*/

		if (isFiring) {			
			/*whichWeapon = ownerShip.getCurrentWeapon ();
			startPos = ownerShip.transform.position;
			angle = ownerShip.getAngle ();*/

			startPos = owner.transform.position;
			angle = owner.transform.eulerAngles.z;

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

	public void setOwner(GameObject newOwner) {
		owner = newOwner;
	}

	/*public float getLastShot() {
		return lastShot;
	}*/

	void release() {
		switch (currentWeapon) {
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
						//The plasma ball has hit something or been destroyed.
						if (plasmas [i] == null) {
							continue;
						}
						Plasma plasma = plasmas [i].GetComponent<Plasma> ();
						plasma.release ();
					}
				}
				break;
			}
		}
	}

	public void fireBullet() {

		if (shotTimer [currentWeapon] == null) {
			shotTimer [currentWeapon] = new ShotTimer ();
		}

		if (ammo [currentWeapon] == null) {
			ammo [currentWeapon] = new Ammo (100, 100);

		}



		switch (currentWeapon) {

			case 1:		//Machine gun
			{
				if (MachineGun.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 

				
				GameObject newBullet = (GameObject)Instantiate(MachineGun.getBullet());
				MachineGun mg = newBullet.GetComponent<MachineGun>();				
				mg.init(owner, startPos, angle);				

				NetworkServer.Spawn(newBullet);			

				break;	
			}

			case 2:		//Rockets
			{
				
				if (Rocket.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 
				
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
				if (Missile.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 
				
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
				//No refire rate.  It's ON / OFF

				if (ammo [currentWeapon].useAmmo() == false) {
					release ();
					return;
				} 

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
				if (Crush.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 
				
				float newAngle = angle;
				GameObject newBullet = (GameObject)Instantiate(Crush.getBullet());
				Crush crush = newBullet.GetComponent<Crush>();

				crush.init(owner, startPos, newAngle);	

				NetworkServer.Spawn(newBullet);						

				break;
			}

			case 6:		//Nuke
			{
				if (Nuke.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 
				
				GameObject newBomb = (GameObject)Instantiate(Nuke.getBomb());
				Nuke nuke = newBomb.GetComponent<Nuke>();
				nuke.init(owner);
				
				NetworkServer.Spawn(newBomb);
				
				break;	
			}

			case 7:		//Warp
			{
				if (Warp.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 

				GameObject newWarp = (GameObject)Instantiate(Warp.getWarp());
				Warp warp = newWarp.GetComponent<Warp> ();

				warp.setOwner (owner);
				break;
			}

			case 8:		//Plasma
			{			

				float newAngle = 0;
				float angleChange = 0;
				float arc = 0;

				//Check to see if it's the first shot.
				//If not, the plasma has already been activated and it should charge
				if (firstShot == false) {					
					//Increase the Plasma's charge

					if (chargingPlasmas != null) {
						//Get the array of Plasmas
						//GameObject[] plasmas = chargingPlasmas.getObjects ();

						List<GameObject> plasmas = chargingPlasmas.getObjectList ();

						for (int i = 0; i < plasmas.Count; i++) {
							//The plasma ball has hit something or is destroyed
							if (plasmas [i] == null) {
								continue;
							}

							//Get the plasma and increase its charge
							Plasma plasma = plasmas [i].GetComponent<Plasma> ();
							plasma.incCharge (Time.deltaTime);

							//If it is the first plasma ball, figure out the angle to increase each shot by.
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

				if (Plasma.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 


				//firstShot is true, set it to false before continuing
				firstShot = false;

				if (chargingPlasmas == null) {
					chargingPlasmas = new ObjectList ();
				}

				//Clear the list so that plasmas that are already fired are not affected
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

			case 9:		//Mines
			{
				if (MineField.getRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].getLastShot())) {
					return;
				}
				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo() == false) {
					return;
				} 

				GameObject newMineField = (GameObject)Instantiate(MineField.getMineField());
				MineField mf = newMineField.GetComponent<MineField>();

				mf.init(owner, startPos);

				NetworkServer.Spawn(newMineField);	
				break;
			}

			case 10:	//Decoy
			{
				/*if (Decoy.getRefireRate () > (Time.fixedTime - shotTimer [currentWeapon].getLastShot ())) {
					return;
				}

				shotTimer [currentWeapon].updateLastShot ();

				if (ammo [currentWeapon].useAmmo () == false) {
					return;
				}

				GameObject newDecoy = (GameObject)Instantiate (Decoy.getDecoy ());
				Decoy d = newDecoy.GetComponent<Decoy> ();

				d.init (owner, ownerShip);

				NetworkServer.Spawn (newDecoy);
				*/

				break;
			}

			case 11:	//Deactivator
			{
				break;
			}

			case 12:	//Turrets
			{
				break;
			}
		}
	}

	public void setCurrentWeapon(int whichWeapon) {
		if (currentWeapon >= 0) {
			currentWeapon = whichWeapon;
		}
	}

	public int getCurrentWeapon() {
		return currentWeapon;
	}

	public float getLastShot(int which) {

		if (which < 0 || which > shotTimer.Length) {
			return -1;
		}

		if (shotTimer [which] != null) {
			return shotTimer [which].getLastShot ();
		} else {
			return 0;
		}
	}
}
