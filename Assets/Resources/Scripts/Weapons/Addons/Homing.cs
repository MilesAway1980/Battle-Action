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
				GetTarget ();
			} else {
				ChaseTarget ();
			}
		}
	}


	void GetTarget() {
		if (isServer) {		
			List<GameObject> targets = Damageable.damageableList.GetObjectList();

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

	void ChaseTarget() {
		if (isServer) {
			Bullet thisBullet = GetComponent<Bullet> ();
		
			float currentAngle = transform.eulerAngles.z;
			float angleToTarget = 360 - Angle.GetAngle (transform.position, target.transform.position);
		
			int turnDir = Angle.GetDirection (currentAngle, angleToTarget);
		
			thisBullet.ChangeAngle(turnRate * turnDir);
		
			//Lose target if it gets out of range.
			if (Vector2.Distance (transform.position, target.transform.position) > detectDistance) {
				target = null;
			}
		}
	}

	public void SetOwner(GameObject newOwner) {
		owner = newOwner;
	}

	public float GetTurnRate() {
		return turnRate;
	}

	public void SetTurnRate(float newTurnRate) {
		if (newTurnRate >= 0) {
			turnRate = newTurnRate;
		}
	}
}
