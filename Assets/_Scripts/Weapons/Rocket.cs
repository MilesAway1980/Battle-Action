using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class Rocket : Bullet
{

	public int range;
	static ShootingInfo rocketInfo = null;

	// Use this for initialization
	void Start()
	{

		if (ownerGuid == Guid.Empty)
		{
			return;
		}

		travelDist = range;

		rigidBody.velocity = new Vector2(
			-Mathf.Sin(angleRad) * speed,
			Mathf.Cos(angleRad) * speed
		);
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

		rigidBody.velocity = new Vector3(
			-Mathf.Sin(angleRad) * speed,
			Mathf.Cos(angleRad) * speed,
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
			
			if (dm && hitOwner)
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

	/*void CheckHit() {
		GameObject objectHit = CheckObjectHit(true);
		if (objectHit) {
			Damageable dm = objectHit.GetComponent<Damageable>();
			if (dm) {
				dm.Damage (damage);
				HitInfo info = objectHit.GetComponent<HitInfo>();
				if (info) {
					info.SetLastHitBy (owner);
				}
				Destroy (gameObject);
			}
		}
	}*/

	public static GameObject GetBulletPrefab()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Projectiles/RocketBullet");
	}

	public new static float GetRefireRate()
	{
		if (rocketInfo == null)
		{
			CreateRocketInfo();
		}
		return rocketInfo.refireRate;
	}

	public new static int GetBulletsPerShot()
	{
		if (rocketInfo == null)
		{
			CreateRocketInfo();
		}
		return rocketInfo.bulletsPerShot;
	}

	static void CreateRocketInfo()
	{
		Rocket temp = GetBulletPrefab().GetComponent<Rocket>();
		rocketInfo = new ShootingInfo();
		rocketInfo.bulletsPerShot = temp.bulletsPerShot;
		rocketInfo.refireRate = temp.refireRate;
	}
}
