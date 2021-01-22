using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ShootingInfo {
	public float refireRate = -1;
	public float bulletsPerShot = -1;
}

public class Bullet : NetworkBehaviour {

	public float speed;
	public float damage;
	public float refireRate;
	public float bulletsPerShot;

	protected GameObject owner;

	[SyncVar] protected Vector2 originPos;
	[SyncVar] protected Vector2 pos;
	[SyncVar] protected Vector2 prevPos;
	[SyncVar] protected float angleRad;
	[SyncVar] protected float angleDeg;

	protected float travelDist;
	protected float distance;

	protected Rigidbody2D rigidBody;
	protected CircleCollider2D circleCollider;

	[Server]
	public void Init(GameObject newOwner, Vector2 startPos, float newAngle) {

		angleDeg = Angle.FixAngle(newAngle);

		angleRad = angleDeg / Mathf.Rad2Deg;

		owner = newOwner;
		originPos = startPos;
		pos = startPos;

		//Check if the object is homing
		Homing homing = gameObject.GetComponent<Homing> ();
		if (homing != null) {
			homing.SetOwner(owner);
		}
	}

	void Update() {
		
	}

	protected GameObject CheckObjectHit(bool ignoreOwner) {
		if (isServer) {		

			List<GameObject> targets = Damageable.damageableList.GetObjectList();

			if (targets.Count == 0) {
				return null;
			}

			int ownerNum = -1;

			if (owner) {
				Owner ownerInfo = owner.GetComponent<Owner> ();
				if (ownerInfo) {
					ownerNum = ownerInfo.GetOwnerNum();
				}
			}

			for (int i = 0; i < targets.Count; i++) {
				GameObject target = targets [i].gameObject;
				if (ignoreOwner) {
					Owner targetOwner = target.GetComponent<Owner> ();
					if (targetOwner) {
						if (targetOwner.GetOwnerNum () == ownerNum) {
							continue;
						}
					}
				}

				if (Vector2.Distance (target.transform.position, pos) <= (speed * 2)) {
					if (Intersect.LineCircle (prevPos, pos, target.transform.position, speed)) {
						return target;
					}
				}
			}
		}

		return null;
	}

	public void ChangeAngle(float angleChange) {
		angleDeg = Angle.FixAngle(angleDeg + angleChange);
		angleRad = angleDeg / Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angleDeg));
	}

	public float GetAngle() {
		return angleDeg;
	}

	public static float GetRefireRate() {
		return float.MaxValue;
	}

	public static float GetBulletsPerShot() {
		return -1;
	}
}
