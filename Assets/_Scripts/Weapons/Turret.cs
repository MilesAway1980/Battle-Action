using UnityEngine;
using System.Collections;
using System;

public class Turret : MonoBehaviour
{

	public float refireRate;
	static float turretRefireRate = -1;

	public float turnRate;                  //How fast it turns
	public float detectDistance;            //The range that the turret will detect enemies
	public float ownerDeactivateRange;      //Will not shoot if owner is within this range.  Protects against camping

	GameObject owner;



	// Use this for initialization
	void Start()
	{


	}

	// Update is called once per frame
	void Update()
	{
		FireAtClosestTarget();
	}

	protected void FireAtClosestTarget()
	{
		ObjectList shipList = Ship.shipList;
		GameObject closest = shipList.GetClosest(gameObject);
		BulletShooter bs = GetComponent<BulletShooter>();

		if (closest == null)
		{
			return;
		}

		//Owner's ship has been destroyed
		if (owner == null)
		{
			Guid turretOwner = GetComponent<Owner>().GetOwnerGuid();
			owner = Ship.shipList.GetObjectByOwner(turretOwner);
		}

		if (owner != null)
		{
			float ownerDist = Vector2.Distance(owner.transform.position, transform.position);
			//Do not shoot if owner is close.  
			if (ownerDist <= ownerDeactivateRange)
			{
				return;
			}
		}

		float angleToTarget = Angle.GetAngle(transform.position, closest.transform.position);
		float fixedAngle = 360 - transform.eulerAngles.z;
		float angleDist = Mathf.Abs(angleToTarget - fixedAngle);
		float turnDir = Angle.GetDirection(fixedAngle, angleToTarget, angleDist);

		bool pointingAt = true;

		if (angleDist < 5)
		{
			pointingAt = true;
		}

		if (angleDist < turnRate)
		{
			//transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angleToTarget));
		}

		if (turnDir < 0)
		{
			transform.Rotate(new Vector3(0, 0, turnRate));
		}
		else if (turnDir > 0)
		{
			transform.Rotate(new Vector3(0, 0, -turnRate));
		}

		if (pointingAt)
		{
			float closestDist = Vector2.Distance(transform.position, closest.transform.position);
			if ((closestDist > 0) && (closestDist <= detectDistance))
			{  //Found the SOB!  SHOOT HIM!			
				if (bs)
				{
					bs.SetIsFiring(true);
				}
			}
			else
			{
				if (bs)
				{
					bs.SetIsFiring(false);
				}
			}
		}
	}

	public void Init(GameObject newOwner)
	{
		owner = newOwner;

		Owner turretOwner = GetComponent<Owner>();
		if (turretOwner)
		{
			Owner info = owner.GetComponent<Owner>();
			if (info)
			{
				turretOwner.SetOwnerGuid(info.GetOwnerGuid());
			}
		}

		BulletShooter bs = GetComponent<BulletShooter>();
		if (bs)
		{
			//bs.SetOwner(gameObject);
			bs.SetCurrentWeapon(1);
		}

		Ship ownerShip = owner.GetComponent<Ship>();

		Vector3 pos = ownerShip.transform.position;
		float angleRad = ownerShip.transform.eulerAngles.z * Mathf.Deg2Rad;

		transform.position = new Vector3(
			pos.x - Mathf.Sin(angleRad) * -2,
			pos.y + Mathf.Cos(angleRad) * -2,
			0
		);
	}

	public static GameObject GetTurret()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Turret");
	}

	public static float GetRefireRate()
	{
		if (turretRefireRate == -1)
		{
			turretRefireRate = GetTurret().GetComponent<Turret>().refireRate;
		}

		return turretRefireRate;
	}
}
