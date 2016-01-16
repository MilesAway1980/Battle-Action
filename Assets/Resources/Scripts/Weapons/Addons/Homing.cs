using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Homing : NetworkBehaviour {

	public float turnRate;
	public float detectDistance;

	GameObject owner;
	GameObject target;

	void FixedUpdate () {
		
		if (isServer) {
			if (target == null) {
				getTarget ();
			} else {
				chaseTarget ();
			}
		}
	}


	void getTarget() {
		if (isServer) {		
			List<GameObject> targets = Damageable.damageableList.getObjectList();

			if (targets.Count == 0) {
				return;
			}

			float closest = float.MaxValue;
			for (int i = 0; i < targets.Count; i++) {
				GameObject potentialTarget = targets [i].gameObject;

				if (potentialTarget == owner) {
					continue;
				}

				float distance = Vector2.Distance (potentialTarget.transform.position, transform.position);
				if (distance < closest && distance < detectDistance) {
					closest = distance;
					target = potentialTarget;
				}
			}
		}
	}

	void chaseTarget() {
		if (isServer) {
			Bullet thisBullet = GetComponent<Bullet> ();
		
			float currentAngle = transform.eulerAngles.z;
			float angleToTarget = 360 - Angle.getAngle (transform.position, target.transform.position);
		
			int turnDir = Angle.getDirection (currentAngle, angleToTarget);
		
			thisBullet.changeAngle (turnRate * turnDir);
		
			//Lose target if it gets out of range.
			if (Vector2.Distance (transform.position, target.transform.position) > detectDistance) {
				target = null;
			}
		}
	}

	public void setOwner(GameObject newOwner) {
		owner = newOwner;
	}

	public float getTurnRate() {
		return turnRate;
	}

	public void setTurnRate(float newTurnRate) {
		if (newTurnRate >= 0) {
			turnRate = newTurnRate;
		}
	}
}
