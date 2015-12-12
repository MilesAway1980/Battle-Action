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

	protected Ship owner;
<<<<<<< HEAD:Assets/Resources/Scripts/Bullet.cs
<<<<<<< HEAD:Assets/Resources/Scripts/Weapons/Bullet.cs
	[SyncVar] protected Vector2 originPos;
	[SyncVar] protected Vector2 pos;
	[SyncVar] protected float angleRad;
	[SyncVar] protected float angleDeg;
	//protected int type;
=======
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.:Assets/Resources/Scripts/Bullet.cs
	protected Vector2 originPos;
	protected Vector2 pos;
	protected float angleRad;
	protected float angleDeg;
	protected int type;
<<<<<<< HEAD:Assets/Resources/Scripts/Bullet.cs
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.:Assets/Resources/Scripts/Bullet.cs
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.:Assets/Resources/Scripts/Bullet.cs
	protected float travelDist;

	protected float distance;

	protected Rigidbody2D rigidBody;
	protected CircleCollider2D circleCollider;

	void Awake() {	

	}

	[Server]
	public void init(Ship newOwner, Vector2 startPos, float newAngle, int newType) {

		angleDeg = Angle.fixAngle(newAngle);

		angleRad = angleDeg / Mathf.Rad2Deg;

		owner = newOwner;
		originPos = startPos;
		pos = startPos;

		type = newType;

		//Check if the object is homing
		Homing homing = gameObject.GetComponent<Homing> ();
		if (homing != null) {
			homing.setOwner(owner);
		}

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
