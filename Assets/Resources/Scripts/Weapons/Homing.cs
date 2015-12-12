using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Homing : NetworkBehaviour {

	public float turnRate;
	public float detectDistance;


	Ship target = null;
<<<<<<< HEAD
	Ship owner;


=======

	Ship owner;

	//float targetDist;
	//float currentSpeed;

	//Vector2 lastPos;
	//Vector2 currentPos;

>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
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
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
			if (players == null) {
				return;
			}
		
			float closest = float.MaxValue;
			for (int i = 0; i < players.Length; i++) {

				Ship potentialTarget = players [i].GetComponent<Ship> ();
				if (potentialTarget == null) {
					continue;
				}
			
				//Don't chase yourself!
				if (potentialTarget == owner) {
					continue;
				}

				if (potentialTarget != null) {
					float distance = Vector2.Distance (potentialTarget.transform.position, transform.position);
					if (distance < closest && distance < detectDistance) {
						closest = distance;
						target = potentialTarget;
					}
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

	public void setOwner(Ship newOwner) {
		owner = newOwner;
	}
}
