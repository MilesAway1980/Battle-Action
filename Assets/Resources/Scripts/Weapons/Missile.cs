using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Missile : Bullet {
	
	static ShootingInfo missileInfo = null;

	public float speedMultiplier;
	public float initialSpeed;
	float currentSpeed;

	float maxTurnRate;
	
	void Start () {

		currentSpeed = initialSpeed;

		travelDist = ArenaInfo.getArenaSize () * 1.0f;
		if (travelDist < ArenaInfo.getMinBulletTravelDist ()) {
			travelDist = ArenaInfo.getMinBulletTravelDist ();
		}
						
		float forwardX = originPos.x - Mathf.Sin (angleRad) * 2;
		float forwardY = originPos.y + Mathf.Cos (angleRad) * 2;
		
		originPos = new Vector2 (forwardX, forwardY);
		
		transform.position = originPos;
		pos = originPos;

		transform.Rotate( new Vector3 (0, 0, angleDeg));

		maxTurnRate = GetComponent<Homing> ().getTurnRate ();
	}
	
	void FixedUpdate() {

		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

		checkHit ();
		prevPos = pos;

		if (currentSpeed < speed) {
			currentSpeed *= speedMultiplier;
			if (currentSpeed > speed) {
				currentSpeed = speed;
			}
		}

		float turnRate = (currentSpeed / speed) * maxTurnRate;
		GetComponent<Homing> ().setTurnRate (turnRate);
		
		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * currentSpeed,
			pos.y + Mathf.Cos (angleRad) * currentSpeed
		);

		transform.position = pos;
	}

	void checkHit() {
		GameObject objectHit = checkObjectHit (true);
		if (objectHit) {
			//SoundPlayer.PlayClip(hitSound);
			Damageable dm = objectHit.GetComponent<Damageable>();

			if (dm) {
				dm.damage (damage);
				HitInfo info = objectHit.GetComponent<HitInfo> ();
				if (info) {
					info.setLastHitBy (owner);
				}
			}

			Destroy (gameObject);
		}	
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Missile");
	}
	
	public new static float getRefireRate() {
		if (missileInfo == null) {
			createMissileInfo();
		}
		return missileInfo.refireRate;
	}
	
	public new static float getBulletsPerShot() {
		if (missileInfo == null) {
			createMissileInfo();
		}
		return missileInfo.bulletsPerShot;
	}
	
	static void createMissileInfo() {
		Missile temp = Missile.getBullet ().GetComponent<Missile>();
		missileInfo = new ShootingInfo();
		missileInfo.bulletsPerShot = temp.bulletsPerShot;
		missileInfo.refireRate = temp.refireRate;
	}
}
