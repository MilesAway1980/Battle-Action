using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BulletShooter : NetworkBehaviour {

	GameObject owner;

	[SyncVar] bool active;			//If the BulletShooter can currently shoot
	[SyncVar] int currentWeapon;	//Which weapon the ship is equipped with
	Vector2 startPos;				//The weapon's starting position
	float angle;					//The weapon's starting angle

	bool isFiring;					//Used to keep track if the bullet shooter is firing/charging, or should be reset
	bool firstShot;					//Keeps track if the shot is the very first shot of a rapid fire
	float shootTimer;				//How long the shooter has been firing in rapid fire

	GameObject blasterObject;		//The object used to fire the Blaster laser
	ObjectList chargingPlasmas;		//A list used to contain all of the ship's Plasma balls
	GameObject deactivatorBeam;

	public ShotTimer[] shotTimer;			//The ShotTimers for each weapon
	Ammo[] ammo;

	float prevShot;

	void Start() {
		isFiring = false;
		firstShot = true;
		active = true;
		shootTimer = 0;
		shotTimer = new ShotTimer [WeaponInfo.GetWeaponCount () + 1];
		ammo = new Ammo[WeaponInfo.GetWeaponCount () + 1];

		for (int i = 0; i < WeaponInfo.GetWeaponCount(); i++) {
			shotTimer [i] = new ShotTimer ();
			ammo [i] = new Ammo (100, 100);
		}
	}

	void FixedUpdate() {

		if (owner == null) { 
			return;
		}

		if (isFiring) {
			if (active) {
				startPos = owner.transform.position;
				angle = owner.transform.eulerAngles.z;

				FireBullet ();
				shootTimer += Time.deltaTime;
			}
		} else {
			firstShot = true;
			if (shootTimer != 0) {
				Release ();	
				shootTimer = 0;
			}
		}
	}

	public void SetIsFiring(bool newIsFiring) {
		isFiring = newIsFiring;
	}

	public void SetOwner(GameObject newOwner) {
		owner = newOwner;
	}

	void Release() {

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
					GameObject[] plasmas = chargingPlasmas.GetObjects ();
					for (int i = 0; i < plasmas.Length; i++) {
						//The plasma ball has hit something or been destroyed.
						if (plasmas [i] == null) {
							continue;
						}
						Plasma plasma = plasmas [i].GetComponent<Plasma> ();
						plasma.Release ();
					}
				}
				break;
			}
		}
	}

	public void FireBullet() {

		/*if (shotTimer [currentWeapon] == null) {
			shotTimer [currentWeapon] = new ShotTimer ();
		}

		if (ammo [currentWeapon] == null) {
			ammo [currentWeapon] = new Ammo (100, 100);
		}*/

		switch (currentWeapon) {

			case 1:		//Machine gun
			{
				if (MachineGun.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 

				
				GameObject newBullet = Instantiate(MachineGun.GetBullet());
				MachineGun mg = newBullet.GetComponent<MachineGun>();				
				mg.Init(owner, startPos, angle);				

				NetworkServer.Spawn(newBullet);			

				break;	
			}

			case 2:		//Rockets
			{
				
				if (Rocket.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 
				
				float newAngle = angle;
				for (int i = 0; i < Rocket.GetBulletsPerShot(); i++) {
					newAngle += (360.0f / (float)Rocket.GetBulletsPerShot());
					GameObject newBullet = Instantiate(Rocket.GetBullet());
					Rocket rocket = newBullet.GetComponent<Rocket>();				
					rocket.Init(owner, startPos, newAngle);					
					NetworkServer.Spawn(newBullet);						
				}
				break;
			}

			case 3:		//Missile
			{
				if (Missile.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 
				
				float newAngle = angle;
				for (int i = 0; i < Missile.GetBulletsPerShot(); i++) {
					newAngle += (360.0f / (float)Missile.GetBulletsPerShot());
					GameObject newBullet = Instantiate(Missile.GetBullet());
					Missile missile = newBullet.GetComponent<Missile>();				
					missile.Init(owner, startPos, newAngle);					
					NetworkServer.Spawn(newBullet);						
				}

				break;
			}

			case 4:		//Blaster
			{
				//No refire rate.  It's ON / OFF

				if (ammo [currentWeapon].UseAmmo() == false) {
					Release ();
					return;
				} 

				if (blasterObject == null) {
					blasterObject = Instantiate(Blaster.getBlaster());
					NetworkServer.Spawn(blasterObject);
				}

				Blaster blaster = blasterObject.GetComponent<Blaster>();
				blaster.Init(startPos, angle, owner);
				
				break;
			}

			case 5:		//Crush / Flak
			{
				if (Crush.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 
				
				float newAngle = angle;
				GameObject newBullet = Instantiate(Crush.GetBullet());
				Crush crush = newBullet.GetComponent<Crush>();

				crush.Init(owner, startPos, newAngle);	

				NetworkServer.Spawn(newBullet);						

				break;
			}

			case 6:		//Nuke
			{
				if (Nuke.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 
				
				GameObject newBomb = Instantiate(Nuke.GetBomb());
				Nuke nuke = newBomb.GetComponent<Nuke>();
				nuke.Init(owner);
				
				NetworkServer.Spawn(newBomb);
				
				break;	
			}

			case 7:		//Warp
			{
				if (Warp.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 

				GameObject newWarp = Instantiate(Warp.GetWarp());
				Warp warp = newWarp.GetComponent<Warp> ();

				warp.SetOwner (owner);
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

						List<GameObject> plasmas = chargingPlasmas.GetObjectList ();

						for (int i = 0; i < plasmas.Count; i++) {
							//The plasma ball has hit something or is destroyed
							if (plasmas [i] == null) {
								continue;
							}

							//Get the plasma and increase its charge
							Plasma plasma = plasmas [i].GetComponent<Plasma> ();
							plasma.IncreaseCharge(Time.deltaTime);

							//If it is the first plasma ball, figure out the angle to increase each shot by.
							if (i == 0) {
								arc = plasma.GetArc ();
								newAngle = angle - (arc / 2.0f);
								if (Plasma.GetBulletsPerShot () > 1) {
									angleChange = (arc / (Plasma.GetBulletsPerShot () - 1));
								} else {
									angleChange = 0;
									newAngle = angle;
								}
							}

							plasma.SetAngle (newAngle);
							newAngle += angleChange;
						}
					}
					return;
				}

				if (Plasma.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 


				//firstShot is true, set it to false before continuing
				firstShot = false;

				if (chargingPlasmas == null) {
					chargingPlasmas = new ObjectList ();
				}

				//Clear the list so that plasmas that are already fired are not affected
				chargingPlasmas.ClearList ();		

				for (int i = 0; i < Plasma.GetBulletsPerShot(); i++) {

					GameObject newBullet = Instantiate (Plasma.GetBullet ());
					Plasma plasma = newBullet.GetComponent<Plasma> ();
					plasma.SetRadius (0);

					chargingPlasmas.AddObject (newBullet);

					if (i == 0) {
						arc = plasma.GetArc ();
						newAngle = angle - (arc / 2.0f);
						if (Plasma.GetBulletsPerShot () > 1) {
							angleChange = (arc / (Plasma.GetBulletsPerShot () - 1));
						} else {
							angleChange = 0;
							newAngle = angle;
						}
					}

					plasma.Init(owner, startPos, newAngle);					
					NetworkServer.Spawn(newBullet);

					newAngle += angleChange;
				}
				break;
			}

			case 9:		//Mines
			{
				if (MineField.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot())) {
					return;
				}
				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo() == false) {
					return;
				} 

				GameObject newMineField = Instantiate(MineField.GetMineField());
				MineField mf = newMineField.GetComponent<MineField>();

				mf.Init(owner, startPos);

				NetworkServer.Spawn(newMineField);	
				break;
			}

			case 10:	//Decoy
			{
				if (Decoy.GetRefireRate() > (Time.fixedTime - shotTimer [currentWeapon].GetLastShot())) {
					return;
				}

				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo () == false) {
					return;
				}

				GameObject newDecoy = Instantiate (Decoy.GetDecoy ());
				Decoy d = newDecoy.GetComponent<Decoy> ();

				d.Init (owner);

				NetworkServer.Spawn (newDecoy);

				break;
			}

			case 11:	//Turret
			{
				if (Turret.GetRefireRate () > (Time.fixedTime - shotTimer [currentWeapon].GetLastShot())) {
					return;
				}

				shotTimer[currentWeapon].UpdateLastShot();

				if (ammo[currentWeapon].UseAmmo() == false) {
					return;
				}

				GameObject newTurret = Instantiate (Turret.GetTurret ());
				Turret t = newTurret.GetComponent<Turret> ();

				t.Init (owner);

				NetworkServer.Spawn (newTurret);

				break;
			}

			case 12:	//Deactivator
			{
				if (Deactivator.GetRefireRate () > (Time.fixedTime - shotTimer [currentWeapon].GetLastShot ())) {
					return;
				}

				shotTimer [currentWeapon].UpdateLastShot ();

				if (ammo [currentWeapon].UseAmmo () == false) {
					return;
				}

				GameObject newDeactivator = Instantiate (Deactivator.GetTurret ());
				Turret t = newDeactivator.GetComponent<Turret> ();

				t.Init (owner);

				NetworkServer.Spawn (newDeactivator);

				break;
			}

			case 13:	//Deactivator beam
			{
				if (deactivatorBeam == null) {
					GameObject newBeam = Instantiate (DeactivatorBeam.getDeactivatorBeam ());
					DeactivatorBeam db = newBeam.GetComponent<DeactivatorBeam> ();
					db.Init (owner);

					deactivatorBeam = newBeam;

					NetworkServer.Spawn (newBeam);
				}

				break;
			}
		}
	}

	public void SetActive(bool which) {
		active = which;
	}

	public void SetCurrentWeapon(int whichWeapon) {
		if (currentWeapon >= 0) {
			currentWeapon = whichWeapon;
		}
	}

	public int GetCurrentWeapon() {
		return currentWeapon;
	}

	public float GetLastShot(int which) {
		if (shotTimer == null) {
			return -1;
		}

		if (which < 0 || which > shotTimer.Length) {
			return -2;
		}

		if (shotTimer [which] != null) {
			return shotTimer[which].GetLastShot ();
		} else {
			return -3;
		}
	}
}
