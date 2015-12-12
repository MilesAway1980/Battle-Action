using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Homing : NetworkBehaviour {

	public float turnRate;
	public float detectDistance;


<<<<<<< HEAD
<<<<<<< HEAD
	Ship target = null;
<<<<<<< HEAD
	Ship owner;
=======
	public Ship target = null;
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
=======
	Ship target = null;
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.

	Ship owner;

<<<<<<< HEAD
=======

	Ship owner;

=======
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
	//float targetDist;
	//float currentSpeed;

	//Vector2 lastPos;
	//Vector2 currentPos;

<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
=======
	[Server]
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
	void FixedUpdate () {

		if (target == null) {
			getTarget();
		} else {
			chaseTarget();
		}
		//lastPos = currentPos;
		//currentPos = transform.position;
		//currentSpeed = Vector2.Distance (lastPos, currentPos);
	}


	void getTarget() {
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

	void chaseTarget() {
		Bullet thisBullet = GetComponent<Bullet>();
		
		float currentAngle = transform.eulerAngles.z;
		float angleToTarget = 360 - Angle.getAngle(transform.position, target.transform.position);
		
		int turnDir = Angle.getDirection(currentAngle, angleToTarget);
		
		thisBullet.changeAngle(turnRate * turnDir);
		
		//Lose target if it gets out of range.
		//targetDist = Vector2.Distance(transform.position, target.transform.position);
		if (Vector2.Distance(transform.position, target.transform.position) > detectDistance) {
			target = null;
		}

	}

	public void setOwner(Ship newOwner) {
		owner = newOwner;
	}
}
