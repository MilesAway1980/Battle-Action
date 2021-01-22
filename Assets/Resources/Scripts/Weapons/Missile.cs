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

		travelDist = ArenaInfo.GetArenaSize () * 1.0f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist ()) {
			travelDist = ArenaInfo.GetMinBulletTravelDist ();
		}
						
		float forwardX = originPos.x - Mathf.Sin (angleRad) * 2;
		float forwardY = originPos.y + Mathf.Cos (angleRad) * 2;
		
		originPos = new Vector2 (forwardX, forwardY);
		
		transform.position = originPos;
		pos = originPos;

		transform.Rotate( new Vector3 (0, 0, angleDeg));

		maxTurnRate = GetComponent<Homing>().GetTurnRate ();
	}
	
	void FixedUpdate() {

		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

		CheckHit ();
		prevPos = pos;

		if (currentSpeed < speed) {
			currentSpeed *= speedMultiplier;
			if (currentSpeed > speed) {
				currentSpeed = speed;
			}
		}

		float turnRate = (currentSpeed / speed) * maxTurnRate;
		GetComponent<Homing>().SetTurnRate (turnRate);
		
		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * currentSpeed,
			pos.y + Mathf.Cos (angleRad) * currentSpeed
		);

		transform.position = pos;
	}

	void CheckHit() {
		GameObject objectHit = CheckObjectHit(true);
		if (objectHit) {
			//SoundPlayer.PlayClip(hitSound);
			Damageable dm = objectHit.GetComponent<Damageable>();

			if (dm) {
				dm.Damage (damage);
				HitInfo info = objectHit.GetComponent<HitInfo> ();
				if (info) {
					info.SetLastHitBy (owner);
				}
			}

			Destroy (gameObject);
		}	
	}

	public static GameObject GetBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Missile");
	}
	
	public new static float GetRefireRate() {
		if (missileInfo == null) {
			CreateMissileInfo();
		}
		return missileInfo.refireRate;
	}
	
	public new static float GetBulletsPerShot() {
		if (missileInfo == null) {
			CreateMissileInfo();
		}
		return missileInfo.bulletsPerShot;
	}
	
	static void CreateMissileInfo() {
		Missile temp = GetBullet ().GetComponent<Missile>();
		missileInfo = new ShootingInfo();
		missileInfo.bulletsPerShot = temp.bulletsPerShot;
		missileInfo.refireRate = temp.refireRate;
	}
}
