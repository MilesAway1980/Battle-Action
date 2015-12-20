﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShootingInfo {
	public float refireRate = -1;
	public float bulletsPerShot = -1;
}

public class Bullet : NetworkBehaviour {

	public float speed;
	public float damage;
	public float refireRate;
	public float bulletsPerShot;

	protected Player owner;
	protected Ship ownerShip;

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
	public void init(Player newOwner, Vector2 startPos, float newAngle) {

		angleDeg = Angle.fixAngle(newAngle);

		angleRad = angleDeg / Mathf.Rad2Deg;

		owner = newOwner;
		originPos = startPos;
		pos = startPos;

		//Check if the object is homing
		Homing homing = gameObject.GetComponent<Homing> ();
		if (homing != null) {
			homing.setOwner(owner);
		}
	}

	void Update() {
		if (owner != null) {
			if (ownerShip == null) {
				ownerShip = owner.getShip ();
			}
		}
	}

	protected Ship checkShipHit(bool ignoreOwner) {
		if (isServer) {		

			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
		
			if (players == null) {
				return null;
			}

			int ownerNum = owner.getPlayerNum ();
		
			for (int i = 0; i < players.Length; i++) {
				Ship playerShip = players [i].GetComponent<Ship> ();

				if (ignoreOwner) {
					if (ownerNum == playerShip.getOwnerNum()) {
						continue;
					}
				}
			
				if (Vector2.Distance (playerShip.transform.position, pos) <= (speed * 2)) {
					if (Intersect.LineCircle (prevPos, pos, playerShip.transform.position, speed)) {
						return playerShip;
					}
				}
			}
		}

		return null;
	}

	public void changeAngle(float angleChange) {
		angleDeg = Angle.fixAngle(angleDeg + angleChange);
		angleRad = angleDeg / Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angleDeg));
	}

	public float getAngle() {
		return angleDeg;
	}

	public static float getRefireRate() {
		return float.MaxValue;
	}

	public static float getBulletsPerShot() {
		return -1;
	}
}
