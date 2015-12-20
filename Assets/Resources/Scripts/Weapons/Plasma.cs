using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Plasma : Bullet {

	static ShootingInfo plasmaInfo;
	public float maxDamage;
	public float maxChargeTime;
	public float initialCharge;

	[Range (0, 180)]
	public float arc;

	[SyncVar] float charge;
	[SyncVar] float radius;
	bool released;
	static float lastShot;

	//Vector2 pivot;

	// Use this for initialization
	void Start () {

		released = false;
		charge = initialCharge;
		radius = 0;

		travelDist = ArenaInfo.getArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.getMinBulletTravelDist()) {
			travelDist = ArenaInfo.getMinBulletTravelDist();
		}

		//pivot = originPos;

		setPosition ();
	}

	void setPosition() {

		if (ownerShip == null) {
			return;
		}

		float forwardX = ownerShip.transform.position.x - Mathf.Sin (angleRad) * 2;
		float forwardY = ownerShip.transform.position.y + Mathf.Cos (angleRad) * 2;

		originPos = new Vector2 (forwardX, forwardY);

		transform.position = originPos;
		pos = originPos;
	}
	
	void FixedUpdate () {

		if (owner != null) {
			if (ownerShip == null) {			
				ownerShip = owner.getShip ();
				if (ownerShip == null) {
					return;
				}
			}
		}
		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

		setRadius (charge / 5.0f);

		checkHit ();

		prevPos = pos;

		if (released) {
			pos = new Vector2 (
				pos.x - Mathf.Sin (angleRad) * speed,
				pos.y + Mathf.Cos (angleRad) * speed
			);
		}

		transform.position = pos;	
	}

	void checkHit() {
		Ship shipHit = checkShipHit (true);
		if (shipHit != null) {
			//SoundPlayer.PlayClip(hitSound);

			float chargeDamage = (charge / (maxChargeTime + initialCharge)) * maxDamage;
			//print (chargeDamage);

			shipHit.damage(chargeDamage);
			shipHit.setLastHitBy (owner.getPlayerNum ());
			Destroy (gameObject);
		}	
	}

	public void incCharge(float amount) {
		if (amount > 0) {
			charge += amount;
			if (charge > (maxChargeTime + initialCharge)) {
				charge = (maxChargeTime + initialCharge);
			}
		}
	}

	public void incRadius (float amount) {
		if (amount > 0) {
			radius += amount;
		}
	}

	public void decRadius (float amount) {
		if (amount > 0) {
			radius -= amount;
		}
	}

	public void setRadius (float newRadius) {
		if (newRadius > 0) {
			radius = newRadius;
		}
	}

	public float getRadius() {
		return radius;
	}

	public void setAngle(float newAngle) {
		angleDeg = newAngle;
		angleRad = angleDeg / Mathf.Rad2Deg;
		setPosition ();
	}

	public void release() {
		released = true;
	}

	public float getArc() {
		return arc;
	}

	public static float getLastShot() {
		return lastShot;
	}

	public static void updateLastShot() {
		lastShot = Time.fixedTime;
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Plasma");
	}

	public new static float getRefireRate() {
		if (plasmaInfo == null) {
			createPlasmaInfo();
		}
		return plasmaInfo.refireRate;
	}

	public new static float getBulletsPerShot() {
		if (plasmaInfo == null) {
			createPlasmaInfo();
		}
		return plasmaInfo.bulletsPerShot;
	}

	static void createPlasmaInfo() {
		Plasma temp = Plasma.getBullet ().GetComponent<Plasma>();
		plasmaInfo = new ShootingInfo();
		plasmaInfo.bulletsPerShot = temp.bulletsPerShot;
		plasmaInfo.refireRate = temp.refireRate;
	}


}
