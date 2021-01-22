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

	// Use this for initialization
	void Start () {

		released = false;
		charge = initialCharge;
		radius = 0;

		travelDist = ArenaInfo.GetArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist()) {
			travelDist = ArenaInfo.GetMinBulletTravelDist();
		}

		SetPosition ();
	}

	void SetPosition() {

		if (owner == null) {
			return;
		}

		float forwardX = owner.transform.position.x - Mathf.Sin (angleRad) * 2;
		float forwardY = owner.transform.position.y + Mathf.Cos (angleRad) * 2;

		originPos = new Vector2 (forwardX, forwardY);

		transform.position = originPos;
		pos = originPos;
	}
	
	void FixedUpdate () {

		if (owner == null) {
			return;
		}

		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

		SetRadius (charge / 5.0f);

		CheckHit ();

		prevPos = pos;

		if (released) {
			pos = new Vector2 (
				pos.x - Mathf.Sin (angleRad) * speed,
				pos.y + Mathf.Cos (angleRad) * speed
			);
		}

		transform.position = pos;	
	}

	void CheckHit() {
		GameObject objectHit = CheckObjectHit(true);

		if (objectHit) {
			float chargeDamage = (charge / (maxChargeTime + initialCharge)) * maxDamage;

			Damageable dm = objectHit.GetComponent<Damageable> ();
			if (dm) {
				dm.Damage (chargeDamage);
				HitInfo info = objectHit.GetComponent<HitInfo> ();
				if (info) {
					info.SetLastHitBy (owner);
				}
			}
		}
	}

	public void IncreaseCharge(float amount) {
		if (amount > 0) {
			charge += amount;
			if (charge > (maxChargeTime + initialCharge)) {
				charge = (maxChargeTime + initialCharge);
			}
		}
	}

	public void IncreaseRadius (float amount) {
		if (amount > 0) {
			radius += amount;
		}
	}

	public void DecreaseRadius (float amount) {
		if (amount > 0) {
			radius -= amount;
		}
	}

	public void SetRadius (float newRadius) {
		if (newRadius > 0) {
			radius = newRadius;
		}
	}

	public float GetRadius() {
		return radius;
	}

	public void SetAngle(float newAngle) {
		angleDeg = newAngle;
		angleRad = angleDeg / Mathf.Rad2Deg;
		SetPosition ();
	}

	public void Release() {
		released = true;
	}

	public float GetArc() {
		return arc;
	}

	public static GameObject GetBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Plasma");
	}

	public new static float GetRefireRate() {
		if (plasmaInfo == null) {
			CreatePlasmaInfo();
		}
		return plasmaInfo.refireRate;
	}

	public new static float GetBulletsPerShot() {
		if (plasmaInfo == null) {
			CreatePlasmaInfo();
		}
		return plasmaInfo.bulletsPerShot;
	}

	static void CreatePlasmaInfo() {
		Plasma temp = GetBullet().GetComponent<Plasma>();
		plasmaInfo = new ShootingInfo();
		plasmaInfo.bulletsPerShot = temp.bulletsPerShot;
		plasmaInfo.refireRate = temp.refireRate;
	}


}
