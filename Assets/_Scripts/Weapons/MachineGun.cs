using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class MachineGun : Bullet
{

	//AudioSource audioSource;
	AudioClip hitSound;
	AudioClip shootSound;

	//float initialSpeed = 0;
	//float thrust = 1.25f;

	public float shotSpread;

	static ShootingInfo machinegunInfo;
	static MachineGun machineGun;

	void Start()
	{
		if (ownerGuid == Guid.Empty)
        {
			return;
        }

		travelDist = ArenaInfo.GetArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist())
		{
			travelDist = ArenaInfo.GetMinBulletTravelDist();
		}

		angleDeg = transform.eulerAngles.z;
		angleRad = angleDeg * Mathf.Deg2Rad;

		//transform.position = originPos;
		//rigidBody.transform.position = transform.position;

		rigidBody.velocity = new Vector2(
			-Mathf.Sin(angleRad) * speed,
			Mathf.Cos(angleRad) * speed
		);
		
		/*
		rigidBody.AddForce(
			new Vector2(
				-Mathf.Sin(angleRad) * speed * 40,
				Mathf.Cos(angleRad) * speed * 40
			)
		);
		*/

		//print(rigidBody.velocity);

		//hitSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Hit", typeof(AudioClip)));
		//shootSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Shoot1", typeof(AudioClip)));

		//SoundPlayer.PlayClip(shootSound);

		currentSpeed = initialSpeed;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!isServer || ownerGuid == Guid.Empty)
		{
			return;
		}

		distance = Vector2.Distance(originPos, transform.position);
		if (distance >= travelDist)
		{
			Destroy(gameObject);
		}

		/*if (currentSpeed < speed)
		{
			currentSpeed += thrust;
			if (currentSpeed > speed)
			{
				currentSpeed = speed;
			}
		}
		
		rigidBody.velocity = new Vector2(
			-Mathf.Sin(angleRad) * currentSpeed,
			Mathf.Cos(angleRad) * currentSpeed
		);*/
		

		//CheckHit ();		
	}

	/*void CheckHit()
	{
		GameObject objectHit = CheckObjectHit(true);

		if (objectHit)
		{
			Damageable dm = objectHit.GetComponent<Damageable>();
			if (dm)
			{
				dm.Damage(damage);

				HitInfo info = objectHit.GetComponent<HitInfo>();
				if (info)
				{
					info.SetLastHitBy(owner);
				}
			}
			Destroy(gameObject);
		}

		Ship shipHit = checkShipHit (true);
		if (shipHit != null) {
			//SoundPlayer.PlayClip(hitSound);
			shipHit.damage(damage);
			shipHit.setLastHitBy(owner.getPlayerNum());
			Destroy (gameObject);
		}
	}*/

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!isServer)
		{
			return;
		}

		if (collision.gameObject != null)
		{
			Ship ship = collision.gameObject.GetComponent<Ship>();
			if (!ship)
			{
				return;
			}

			Damageable dm = ship.GetComponent<Damageable>();
			Owner hitOwner = ship.GetComponent<Owner>();			


			if (hitOwner)
			{
				print(hitOwner.GetOwnerGuid() + "    " + ownerGuid);


				if (hitOwner.GetOwnerGuid() != ownerGuid)
				{
					//ship.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(500, 500));
					dm.Damage(damage);

					HitInfo info = collision.gameObject.GetComponent<HitInfo>();
					if (info)
					{
						info.SetLastHitBy(ownerGuid);
					}

					Destroy(gameObject);
				}
			}
		}
	}

	public static GameObject GetBullet()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Projectiles/MachineGunBullet");
	}

	public static float GetRandomShotSpread()
	{
		if (machinegunInfo == null)
		{
			CreateMachineGunInfo();
		}

		return UnityEngine.Random.Range(-machineGun.shotSpread, machineGun.shotSpread);
	}

	public static new float GetRefireRate()
	{
		if (machinegunInfo == null)
		{
			CreateMachineGunInfo();
		}
		return machinegunInfo.refireRate;
	}

	public static new float GetBulletsPerShot()
	{
		if (machinegunInfo == null)
		{
			CreateMachineGunInfo();
		}
		return machinegunInfo.bulletsPerShot;
	}

	static void CreateMachineGunInfo()
	{
		machineGun = GetBullet().GetComponent<MachineGun>();
		machinegunInfo = new ShootingInfo();
		machinegunInfo.bulletsPerShot = machineGun.bulletsPerShot;
		machinegunInfo.refireRate = machineGun.refireRate;
	}
}
