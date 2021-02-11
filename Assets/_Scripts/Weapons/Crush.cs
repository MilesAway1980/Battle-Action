using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class Crush : Bullet
{

	static ShootingInfo crushInfo;
	public GameObject target;
	public bool hasTarget;

	Vector3 size;
	Vector3 originalSize;

	public float startingSize;
	public float crushSpeed;
	public float startingCrushLevel;
	public float minCrushLevel;

	[SyncVar] float crushLevel;	

	// Use this for initialization
	void Start()
	{
		transform.localScale = Vector3.zero;		//Otherwise it starts huge on the client

		if (ownerGuid == Guid.Empty)
		{
			return;
		}

		travelDist = ArenaInfo.GetArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist())
		{
			travelDist = ArenaInfo.GetMinBulletTravelDist();
		}

		angleRad = angleDeg * Mathf.Deg2Rad;

		rigidBody.velocity = new Vector2(
			-Mathf.Sin(angleRad) * speed,
			Mathf.Cos(angleRad) * speed
		);

		originalSize = new Vector3(
			startingSize,
			startingSize,
			startingSize
		); ;
		size = originalSize;
	}

	// Update is called once per frame
	void FixedUpdate()
	{

		if (!isServer || ownerGuid == Guid.Empty)
		{
			return;
		}

		if (hasTarget == false)
		{
			transform.localScale = originalSize;
			rigidBody.velocity = new Vector3(
				-Mathf.Sin(angleRad) * speed,
				Mathf.Cos(angleRad) * speed,
				0
			);

			distance = Vector2.Distance(originPos, transform.position);
			if (distance >= travelDist)
			{
				Destroy(gameObject);
			}		
		}
		else
        {
			if (target == null)
            {
				hasTarget = false;
				size = originalSize;
            }
			else
            {
				hasTarget = true;
				CrushTarget();
				transform.position = target.transform.position;
            }
        }

		/*if (isServer)
		{
			if (target != null)
			{
				hasTarget = true;
				CrushTarget();
			}
			else
			{
				hasTarget = false;
				size = new Vector3(0.2f, 0.2f, 0.2f);
				CheckHit();
			}
		}*/

		//transform.position = pos;
		transform.localScale = size;
	}

    /*void CheckHit()
	{
		GameObject objectHit = CheckObjectHit(true);
		if (objectHit != null)
		{
			//SoundPlayer.PlayClip(hitSound);
			target = objectHit;
			crushLevel = startingCrushLevel;
		}
	}*/

    private void OnTriggerEnter2D(Collider2D collision)
    {
		Damageable dm = collision.gameObject.GetComponent<Damageable>();
		if (dm)
        {
			Owner damageOwner = collision.gameObject.GetComponent<Owner>();

			if (damageOwner.GetOwnerGuid() != ownerGuid)
			{
				hasTarget = true;
				target = collision.gameObject;
				crushLevel = startingCrushLevel;

				HitInfo hitInfo = collision.gameObject.GetComponent<HitInfo>();
				if (hitInfo)
				{
					hitInfo.SetLastHitBy(ownerGuid);
				}
			}
		}
    }

    [Server]
	void CrushTarget()
	{
		//pos = target.transform.position;
		size = new Vector3(
			crushLevel,
			crushLevel,
			crushLevel
		);
		crushLevel -= crushSpeed;
		Damageable dm = target.GetComponent<Damageable>();
		if (dm)
		{
			dm.Damage(damage);
		}

		if (crushLevel < minCrushLevel)
		{
			crushLevel = minCrushLevel;
			dm.Destruct();
		}
	}

	public static GameObject GetBullet()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Projectiles/Crush");
	}

	public static new float GetRefireRate()
	{
		if (crushInfo == null)
		{
			CreateCrushInfo();
		}
		return crushInfo.refireRate;
	}

	public static new float GetBulletsPerShot()
	{
		if (crushInfo == null)
		{
			CreateCrushInfo();
		}
		return crushInfo.bulletsPerShot;
	}

	static void CreateCrushInfo()
	{
		Crush temp = GetBullet().GetComponent<Crush>();
		crushInfo = new ShootingInfo();
		crushInfo.bulletsPerShot = temp.bulletsPerShot;
		crushInfo.refireRate = temp.refireRate;
	}
}
