using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	public float refireRate;
	static float turretRefireRate = -1;

	public float turnRate;					//How fast it turns
	public float detectDistance;			//The range that the turret will detect enemies
	public float ownerDeactivateRange;		//Will not shoot if owner is within this range.  Protects against camping

	GameObject owner;



	// Use this for initialization
	void Start () {
		

	}
	
	// Update is called once per frame
	void Update () {
		fireAtClosestTarget ();
	}

	protected void fireAtClosestTarget() {
		ObjectList shipList = Ship.shipList;
		GameObject closest = shipList.getClosest (gameObject);
		BulletShooter bs = GetComponent<BulletShooter>();

		if (closest == null) {
			return;
		}

		//Owner's ship has been destroyed
		if (owner == null) {
			int turretOwner = GetComponent<Owner> ().getOwnerNum ();
			owner = Ship.shipList.getObjectByOwner (turretOwner);
		}

		if (owner != null) {
			float ownerDist = Vector2.Distance (owner.transform.position, transform.position);
			//Do not shoot if owner is close.  
			if (ownerDist <= ownerDeactivateRange) {
				return;
			}
		}

		float angleToTarget = Angle.getAngle (transform.position, closest.transform.position);
		float fixedAngle = 360 - transform.eulerAngles.z;
		float angleDist = Mathf.Abs (angleToTarget - fixedAngle);
		float turnDir = Angle.getDirection (fixedAngle, angleToTarget, angleDist);

		bool pointingAt = true;

		if (angleDist < 5) {
			pointingAt = true;
		}

		if (angleDist < turnRate) {
			//transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angleToTarget));
		}

		if (turnDir < 0) {
			transform.Rotate (new Vector3 (0, 0, turnRate));
		} else if (turnDir > 0) {
			transform.Rotate (new Vector3 (0, 0, -turnRate));
		}

		if (pointingAt) {
			float closestDist = Vector2.Distance (transform.position, closest.transform.position);
			if ((closestDist > 0) && (closestDist <= detectDistance)) {  //Found the SOB!  SHOOT HIM!			
				if (bs) {
					bs.setIsFiring (true);
				} 
			} else {
				if (bs) {
					bs.setIsFiring (false);
				}
			}
		}
	}

	public void init(GameObject newOwner) {
		owner = newOwner;

		Owner turretOwner = GetComponent<Owner> ();
		if (turretOwner) {
			Owner info = owner.GetComponent<Owner> ();
			if (info) {
				turretOwner.setOwnerNum (info.getOwnerNum ());
			}
		}

		BulletShooter bs = GetComponent<BulletShooter>();
		if (bs) {
			bs.setOwner (gameObject);
			bs.setCurrentWeapon (1);
		}

		Ship ownerShip = owner.GetComponent<Ship> ();

		Vector3 pos = ownerShip.transform.position;
		float angle = ownerShip.transform.eulerAngles.z;

		transform.position = new Vector3 (
			pos.x - Mathf.Sin(angle / Mathf.Rad2Deg) * -2,
			pos.y + Mathf.Cos(angle / Mathf.Rad2Deg) * -2,
			0
		);
	}

	public static GameObject getTurret() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Turret");
	}

	public static float getRefireRate() {
		if (turretRefireRate == -1) {
			turretRefireRate = getTurret ().GetComponent<Turret> ().refireRate;
		}

		return turretRefireRate;
	}
}
