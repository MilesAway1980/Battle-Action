using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class ShootingInfo
{
	public float refireRate = -1;
	public int bulletsPerShot = -1;
}

public class Bullet : NetworkBehaviour
{
	public float speed;
	public float initialSpeed;
	public float thrust;
	public float damage;
	public float refireRate;
	public int bulletsPerShot;

	protected Guid ownerGuid;
	protected float currentSpeed;

	protected Vector2 originPos;
	
	protected float angleRad;
	protected float angleDeg;

	protected float travelDist;
	protected float distance;

	protected Rigidbody2D rigidBody;	

	[Server]
	public void Init(Guid ownerGuid)
	{
		
		this.ownerGuid = ownerGuid;

		angleDeg = transform.eulerAngles.z;
		angleRad = angleDeg * Mathf.Deg2Rad;

		originPos = transform.position;

		//SetIgnoreColliders();

		rigidBody = GetComponent<Rigidbody2D>();
		
		//Check if the object is homing
		Homing homing = gameObject.GetComponent<Homing>();
		if (homing != null)
		{
			homing.SetOwner(ownerGuid);
		}		
	}

	void Update()
	{
		
	}
	
	/*void SetIgnoreColliders()
	{
		Collider2D[] bulletColliders = GetComponentsInChildren<Collider2D>();
		Collider2D[] ownerColliders = owner.GetComponentsInChildren<Collider2D>();

		print(bulletColliders.Length + "  " + ownerColliders.Length);

		for (int b = 0; b < bulletColliders.Length; b++)
		{
			for (int o = 0; o < ownerColliders.Length; o++)
			{
				print(bulletColliders[b]);
				print(ownerColliders[o]);
				Physics2D.IgnoreCollision(bulletColliders[b], ownerColliders[o]);
			}
		}
	}*/

	/*[Server]
	protected GameObject CheckObjectHit(bool ignoreOwner)
	{
		List<GameObject> targets = Damageable.damageableList.GetObjectList();

		print(targets.Count);

		if (targets.Count == 0)
		{
			return null;
		}

		Guid ownerGuid = Guid.Empty;

		if (owner)
		{
			Owner ownerInfo = owner.GetComponent<Owner>();
			if (ownerInfo)
			{
				ownerGuid = ownerInfo.GetOwnerGuid();
			}
		}

		for (int i = 0; i < targets.Count; i++)
		{
			GameObject target = targets[i].gameObject;
			if (ignoreOwner)
			{
				Owner targetOwner = target.GetComponent<Owner>();
				if (targetOwner)
				{
					if (targetOwner.GetOwnerGuid() == ownerGuid)
					{
						continue;
					}
				}
			}

			if (Vector2.Distance(target.transform.position, pos) <= (speed * 2))
			{
				if (Intersect.LineCircle(prevPos, pos, target.transform.position, speed))
				{
					return target;
				}
			}
		}

		return null;
	}*/

	[Server]
	public void ChangeAngle(float angleChange)
	{
		angleDeg = Angle.FixAngle(angleDeg + angleChange);
		angleRad = angleDeg * Mathf.Deg2Rad;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleDeg));
	}

	[Server]
	public float GetAngle()
	{
		return angleDeg;
	}

	[Server]
	public static float GetRefireRate()
	{
		return float.MaxValue;
	}

	[Server]
	public static float GetBulletsPerShot()
	{
		return -1;
	}
}
