using UnityEngine;
using System.Collections;
using System;
using Mirror;

public class Turret : NetworkBehaviour
{

	public float refireRate;
	static float turretRefireRate = -1;

	[Tooltip("How fast the turret will turn to track enemies")]
	public float turnRate;
	[Tooltip("How far away the turret will detect enemies")]
	public float detectDistance;
	[Tooltip("If the owner is inside this range, the turret will not shoot. Keeps players from camping.")]
	public float ownerDeactivateRange;

	public bool infiniteAmmo;
	[Tooltip("If the turret does not have infinite ammo, this is how many times it can shoot before it self destructs.")]
	public int ammoBeforeExploding;
	int currentAmmo;

	public GameObject turretHead;

	Guid ownerGuid;
	//Owner owner;

	Ship ownerShip;
	float angle;

	public static ObjectList turretList = new ObjectList();

	// Use this for initialization
	void Start()
	{
		turretList.AddObject(gameObject);
		currentAmmo = ammoBeforeExploding;
		if (isServer)
        {
			angle = UnityEngine.Random.Range(0, 360);
			turretHead.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
	}

	private void OnDestroy()
	{
		turretList.RemoveObject(gameObject);
	}

	public void Init(Guid ownerGuid)
	{
		this.ownerGuid = ownerGuid;

		Owner turretOwner = GetComponent<Owner>();
		if (turretOwner)
		{
			turretOwner.SetOwnerGuid(ownerGuid);
		}



		//Ship ownerShip = owner.GetComponent<Ship>();

		//Vector3 pos = ownerShip.transform.position;
		//float angleRad = ownerShip.transform.eulerAngles.z * Mathf.Deg2Rad;

		/*transform.position = new Vector3(
			pos.x - Mathf.Sin(angleRad) * -2,
			pos.y + Mathf.Cos(angleRad) * -2,
			0
		);*/
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!isServer)
		{
			return;
		}

		FireAtClosestTarget();
	}

	protected void FireAtClosestTarget()
	{
		
		//BulletShooter bs = GetComponent<BulletShooter>();

		

		if (ownerShip == null)
        {
			ownerShip = Ship.GetShipByGuid(ownerGuid);
        }

		GameObject[] shipsToIgnore = null;
		if (ownerShip)
		{
			float ownerDist = Vector2.Distance(ownerShip.transform.position, transform.position);
			if (ownerDist <= ownerDeactivateRange)
			{
				//The owner is too close, do nothing
				return;
			}

			shipsToIgnore = new GameObject[1];
			shipsToIgnore[0] = ownerShip.gameObject;
		}

		ObjectList shipList = Ship.shipList;
		GameObject closest = shipList.GetClosest(gameObject, shipsToIgnore);

		if (closest == null)
		{
			return;
		}

		float angleToTarget = Angle.GetAngle(transform.position, closest.transform.position);		
		float angleDist = Mathf.Abs(angleToTarget - angle);
		float turnDir = Angle.GetDirection(angle, angleToTarget);

		if (angleDist <= angleToTarget)
        {
			angle = angleToTarget;
        }
		else
        {
			angle += turnDir * turnRate;
        }


		bool pointingAt = false;
		if (angleDist < 5)
		{
			pointingAt = true;
		}

		turretHead.transform.rotation = Quaternion.Euler(0, 0, angle);

		if (pointingAt)
		{
			float closestDist = Vector2.Distance(transform.position, closest.transform.position);
			if ((closestDist > 0) && (closestDist <= detectDistance))
			{  //Found the SOB!  SHOOT HIM!			
				/*if (bs)
				{
					bs.SetIsFiring(true);
				}*/
				print("SHOOT");
			}
			else
			{
				/*if (bs)
				{
					bs.SetIsFiring(false);
				}
				*/

				print("STOP SHOOT");
			}
		}
	}

	

	public static GameObject GetTurretPrefab()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Turret");
	}

	public static float GetRefireRate()
	{
		if (turretRefireRate == -1)
		{
			turretRefireRate = GetTurretPrefab().GetComponent<Turret>().refireRate;
		}

		return turretRefireRate;
	}
}
