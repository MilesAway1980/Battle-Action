using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class Missile : Bullet
{
	static ShootingInfo missileInfo = null;

	//public float initialSpeed;
	//public float thrust;

	//float currentSpeed;

    void Start()
	{
		if (ownerGuid == Guid.Empty)
        {
			return;
        }		

		travelDist = ArenaInfo.GetArenaSize() * 1.0f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist())
		{
			travelDist = ArenaInfo.GetMinBulletTravelDist();
		}

		rigidBody.velocity = new Vector2(
			-Mathf.Sin(angleRad) * initialSpeed,
			Mathf.Cos(angleRad) * initialSpeed
		);

		currentSpeed = initialSpeed;		
	}

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

		currentSpeed += thrust;
		if (currentSpeed > speed)
        {
			currentSpeed = speed;
        }

		rigidBody.velocity = new Vector3(
			-Mathf.Sin(angleRad) * currentSpeed,
			Mathf.Cos(angleRad) * currentSpeed,
			0
		);
	}

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

    /*[Server]
	void CheckHit()
	{
		GameObject objectHit = CheckObjectHit(true);
		if (objectHit)
		{
			//SoundPlayer.PlayClip(hitSound);
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
	}*/

	[Server]
	public static GameObject GetBulletPrefab()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Projectiles/Missile");
	}

	[Server]
	public new static float GetRefireRate()
	{
		if (missileInfo == null)
		{
			CreateMissileInfo();
		}
		return missileInfo.refireRate;
	}

	[Server]
	public new static float GetBulletsPerShot()
	{
		if (missileInfo == null)
		{
			CreateMissileInfo();
		}
		return missileInfo.bulletsPerShot;
	}

	[Server]
	static void CreateMissileInfo()
	{
		Missile temp = GetBulletPrefab().GetComponent<Missile>();
		missileInfo = new ShootingInfo();
		missileInfo.bulletsPerShot = temp.bulletsPerShot;
		missileInfo.refireRate = temp.refireRate;
	}
}
