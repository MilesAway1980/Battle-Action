using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class BulletShooter : NetworkBehaviour
{
	Owner owner;	

	bool active;					//If the BulletShooter can currently shoot
	[SyncVar] int currentWeapon;    //Which weapon the ship is equipped with
	
	float angle;                    //The weapon's starting angle

	[SyncVar] bool isFiring;        //Used to keep track if the bullet shooter is firing/charging, or should be reset
	bool firstShot;                 //Keeps track if the shot is the very first shot of a rapid fire
	float shootTimer;               //How long the shooter has been firing in rapid fire

	GameObject blasterObject;       //The object used to fire the Blaster laser
	ObjectList chargingPlasmas;     //A list used to contain all of the ship's Plasma balls
	GameObject deactivatorBeam;

	public ShotTimer[] shotTimer;   //The ShotTimers for each weapon

	readonly SyncDictionary<int, Ammo> ammo = new SyncDictionary<int, Ammo>();
	
	bool initialized = false;
		
	void Start()
	{
		isFiring = false;
		firstShot = true;
		active = true;
		shootTimer = 0;
		shotTimer = new ShotTimer[WeaponInfo.GetWeaponCount()];
		
		owner = GetComponent<Owner>();		

		chargingPlasmas = new ObjectList();
	}
	
	void FixedUpdate()
	{
		if (!isServer)
		{
			return;
		}

		if (isFiring)
		{
			if (active)
			{
				angle = transform.eulerAngles.z;

				FireBullet();
				shootTimer += Time.deltaTime;
			}
		}
		else
		{
			firstShot = true;
			if (shootTimer != 0)
			{
				Release();
				shootTimer = 0;
			}
		}
	}

	[Server]
	public void InitializeAmmo(float modifier)
	{
		if (initialized)
		{
			return;
		}

		if (shotTimer == null)
        {
			return;
        }

		initialized = true;
		modifier = modifier == 0 ? 0.1f : modifier;

		for (int i = 0; i < WeaponInfo.GetWeaponCount(); i++)
		{
			shotTimer[i] = new ShotTimer();
			shotTimer[i].ReadyShot();

			WeaponData weapon = ArenaInfo.GetWeaponData(i);

			int weaponMax = (int)(weapon.maxAmmo * modifier);
			int startAmount = 0;

			switch (weapon.startingAmmoType)
			{
				case 0: //Empty
					startAmount = 0;
					break;
				case 1: //Full
					startAmount = weaponMax;
					break;
				case 2: //Amount
					startAmount = (int)weapon.startingAmmo;
					break;
				case 3: //Percent
					startAmount = (int)(weaponMax * weapon.startingAmmo / 100.0f);
					break;
			}

			ammo.Add(i, new Ammo(startAmount, weaponMax));
		}
	}

	[Server]
	public void SetIsFiring(bool newIsFiring)
	{
		isFiring = newIsFiring;
	}

	[Server]
	public void SetOwner(Owner newOwner)
    {
		owner = newOwner;
    }

	[Server]
	void Release()
	{
		switch (currentWeapon)
		{
			case 3: //Blaster			
				if (blasterObject != null)
				{
					Destroy(blasterObject);
				}
				break;

			case 7: //Plasma			
				if (chargingPlasmas != null)
				{
					GameObject[] plasmas = chargingPlasmas.GetObjectList().ToArray();
					for (int i = 0; i < plasmas.Length; i++)
					{
						//The plasma ball has hit something or been destroyed.
						if (plasmas[i] == null)
						{
							continue;
						}

						Plasma plasma = plasmas[i].GetComponent<Plasma>();
						plasma.Release();
					}
				}
				break;
		}
	}

	[Server]
	public void FireBullet()
	{
		GameObject newBullet;
		float newAngle;
		float angleChange;
				
		switch (currentWeapon)
		{

			case 0:     //Machine gun			
				if (MachineGun.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				newBullet = Instantiate(
					MachineGun.GetBullet(),
					transform.position,
					Quaternion.Euler(0, 0, angle + MachineGun.GetRandomShotSpread())
				);

				newBullet.GetComponent<MachineGun>().Init(owner.GetOwnerGuid());

				NetworkServer.Spawn(newBullet);

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 1:     //Rockets

				if (Rocket.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				newAngle = angle;
				angleChange = 360.0f / Rocket.GetBulletsPerShot();
				for (int i = 0; i < Rocket.GetBulletsPerShot(); i++)
				{
					if (ammo[currentWeapon].CheckAmmo(1) == false)
					{
						return;
					}

					ammo[currentWeapon].UseAmmo(1);

					newAngle += angleChange;
					newBullet = Instantiate(
						Rocket.GetBullet(),
						//ownerObject.transform.position,
						transform.position,
						Quaternion.Euler(0, 0, newAngle)
					);

					newBullet.GetComponent<Rocket>().Init(owner.GetOwnerGuid());
					NetworkServer.Spawn(newBullet);
				}

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 2:     //Missile
				if (Missile.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				newAngle = angle;
				for (int i = 0; i < Missile.GetBulletsPerShot(); i++)
				{

					if (ammo[currentWeapon].CheckAmmo(1) == false)
					{
						return;
					}

					ammo[currentWeapon].UseAmmo(1);
					newAngle += (360.0f / (float)Missile.GetBulletsPerShot());

					newBullet = Instantiate(
						Missile.GetBullet(),
						//ownerObject.transform.position,
						transform.position,
						Quaternion.Euler(0, 0, newAngle)
					);

					newBullet.GetComponent<Missile>().Init(owner.GetOwnerGuid());
					NetworkServer.Spawn(newBullet);
				}

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 3:     //Blaster
						//No refire rate.  It's ON / OFF

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					Release();
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				if (blasterObject == null)
				{
					blasterObject = Instantiate(
						Blaster.getBlaster(),
						//ownerObject.transform.position,
						transform.position,
						Quaternion.Euler(0, 0, angle)
					);

					NetworkServer.Spawn(blasterObject);
					blasterObject.GetComponent<Blaster>().Init(owner.GetOwnerGuid());
				}

				break;

			case 4:     //Crush / Flak
				if (Crush.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				newAngle = angle;
				newBullet = Instantiate(
					Crush.GetBullet(),
					//ownerObject.transform.position,
					transform.position,
					Quaternion.Euler(0, 0, newAngle)
				);

				Crush crush = newBullet.GetComponent<Crush>();

				crush.Init(owner.GetOwnerGuid());

				NetworkServer.Spawn(newBullet);

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 5:     //Nuke

				if (Nuke.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				GameObject newBomb = Instantiate(Nuke.GetBomb());
				Nuke nuke = newBomb.GetComponent<Nuke>();
				nuke.Init(owner.GetOwnerGuid(), owner.gameObject);

				NetworkServer.Spawn(newBomb);

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 6:     //Warp

				if (Warp.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				GameObject newWarp = Instantiate(Warp.GetWarp());
				Warp warp = newWarp.GetComponent<Warp>();

				//warp.SetOwner(owner.gameObject);
				warp.Init(owner.gameObject);
				NetworkServer.Spawn(newWarp);

				shotTimer[currentWeapon].UpdateLastShot();
				break;

			case 7:     //Plasma

				newAngle = 0;
				int plasmasPerShot = Plasma.GetBulletsPerShot();

				//Check to see if it's the first shot.
				//If not, the plasma has already been activated and it should charge
				if (firstShot == true)
				{
					if (Plasma.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
					{
						return;
					}

					firstShot = false;

					//Clear the list so that plasmas that are already fired are not affected
					chargingPlasmas.ClearList();

					float arc = Plasma.GetArc();
					float halfArc = arc * 0.5f;
					float arcChange = arc / (plasmasPerShot - 1);
					angleChange = 0;

					for (int i = 0; i < plasmasPerShot; i++)
					{
						if (ammo[currentWeapon].CheckAmmo(1) == false)
						{
							continue;
						}

						//Get the start of the arc and the angle change per plasma
						if (i == 0)
						{
							newAngle = angle - halfArc;
							if (plasmasPerShot > 1)
							{
								angleChange = arcChange;
							}
							else
							{
								angleChange = 0;
								newAngle = angle;
							}
						}

						ammo[currentWeapon].UseAmmo(1);

						newBullet = Instantiate(
							Plasma.GetBullet(),
							transform.position,
							Quaternion.Euler(0, 0, newAngle)
						);

						Plasma plasma = newBullet.GetComponent<Plasma>();
						plasma.SetRadius(0);

						chargingPlasmas.AddObject(newBullet);

						plasma.Init(owner.GetOwnerGuid());
						plasma.SetOwnerObject(owner.gameObject);

						NetworkServer.Spawn(newBullet);
						newAngle += angleChange;
					}

					shotTimer[currentWeapon].UpdateLastShot();
				}
				else
				{
					//Increase the Plasma's charge

					if (chargingPlasmas != null)
					{
						//Get the array of Plasmas
						GameObject[] plasmas = chargingPlasmas.GetObjectList().ToArray();

						for (int i = 0; i < plasmas.Length; i++)
						{
							//The plasma ball has hit something or is destroyed
							if (plasmas[i] == null)
							{
								continue;
							}

							//Get the plasma and increase its charge
							Plasma plasma = plasmas[i].GetComponent<Plasma>();

							int ammoUsage = (int)(Time.deltaTime * 100);
							if (ammo[currentWeapon].CheckAmmo(ammoUsage) && plasma.IncreaseCharge(Time.deltaTime))
							{
								ammo[currentWeapon].UseAmmo(ammoUsage);
							}
						}
					}

					return;
				}

				break;

			case 8:     //Mines

				if (MineField.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				GameObject newMineField = Instantiate(
					MineField.GetMineField(),
					transform.position,
					Quaternion.identity
				);

				MineField mf = newMineField.GetComponent<MineField>();

				mf.Init(owner.GetOwnerGuid());

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 9: //Decoy

				if (Decoy.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				float randomStartAngle = Random.Range(0, Angle.DoublePi);
				float startDistance = Random.Range(1.5f, 2.5f);
				Vector3 initPos = new Vector3(
					transform.position.x + Mathf.Cos(randomStartAngle) * startDistance,
					transform.position.y - Mathf.Sin(randomStartAngle) * startDistance,
					0
				);

				GameObject newDecoy = Instantiate(
					Decoy.GetDecoy(),
					initPos,
					Quaternion.Euler(0, 0, Random.Range(0, Angle.DoublePi))
				);

				Decoy decoy = newDecoy.GetComponent<Decoy>();
				decoy.Init(owner.GetOwnerGuid());

				NetworkServer.Spawn(newDecoy);

				shotTimer[currentWeapon].UpdateLastShot();

				break;

			case 10:    //Turret

				if (Turret.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				GameObject newTurret = Instantiate(Turret.GetTurret());
				Turret t = newTurret.GetComponent<Turret>();

				t.Init(owner.gameObject);

				NetworkServer.Spawn(newTurret);

				shotTimer[currentWeapon].UpdateLastShot();

				break;


			case 11:    //Deactivator

				if (Deactivator.GetRefireRate() > (Time.fixedTime - shotTimer[currentWeapon].GetLastShot()))
				{
					return;
				}

				if (ammo[currentWeapon].CheckAmmo(1) == false)
				{
					return;
				}

				ammo[currentWeapon].UseAmmo(1);

				GameObject newDeactivator = Instantiate(Deactivator.GetTurret());
				Turret turret = newDeactivator.GetComponent<Turret>();

				turret.Init(owner.gameObject);

				NetworkServer.Spawn(newDeactivator);

				shotTimer[currentWeapon].UpdateLastShot();

				break;


			case 12:    //Deactivator beam

				if (deactivatorBeam == null)
				{
					GameObject newBeam = Instantiate(DeactivatorBeam.getDeactivatorBeam());
					DeactivatorBeam db = newBeam.GetComponent<DeactivatorBeam>();
					db.Init(owner.gameObject);

					deactivatorBeam = newBeam;

					NetworkServer.Spawn(newBeam);
				}

				break;

		}
	}

	[Server]
	public void DumpAmmo(int whichWeapon)
	{
		if (whichWeapon >= 0 && whichWeapon < ammo.Count)
		{
			ammo[whichWeapon].Empty();
		}
	}

	[Server]
	public int WeaponsWithAmmoCount()
	{
		int count = 0;
		for (int i = 0; i < ammo.Count; i++)
		{
			if (ammo[i].GetCurrentAmmo() > 0)
			{
				count++;
			}
		}

		return count;
	}

	[Server]
	public void SetActive(bool which)
	{
		active = which;
	}

	[Server]
	public void SetCurrentWeapon(int whichWeapon)
	{
		if (currentWeapon >= 0)
		{
			currentWeapon = whichWeapon;
		}
	}

	public int GetCurrentWeapon()
	{
		return currentWeapon;
	}

	[Server]
	public float GetLastShot(int which)
	{
		if (shotTimer == null)
		{
			return -1;
		}

		if (which < 0 || which > shotTimer.Length)
		{
			return -2;
		}

		if (shotTimer[which] != null)
		{
			return shotTimer[which].GetLastShot();
		}
		else
		{
			return -3;
		}
	}

	public Ammo GetWeaponAmmo(int? whichWeapon = null)
	{		
		if (whichWeapon == null)
		{
			whichWeapon = currentWeapon;
		}		

		if (whichWeapon >= 0 && whichWeapon < ammo.Count)
		{
			return ammo[(int)whichWeapon];
		}

		return null;
	}
}
