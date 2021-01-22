using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Rocket : Bullet {

	public int range;
	static ShootingInfo rocketInfo = null;

	// Use this for initialization
	void Start () {

		travelDist = range;

		float forwardX = originPos.x - Mathf.Sin (angleRad) * 2;
		float forwardY = originPos.y + Mathf.Cos (angleRad) * 2;
		
		originPos = new Vector2 (forwardX, forwardY);
		
		transform.position = originPos;
		pos = originPos;

		transform.Rotate( new Vector3 (0, 0, angleDeg));
	}

	void FixedUpdate() {

		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

		CheckHit ();

		prevPos = pos;
		
		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * speed,
			pos.y + Mathf.Cos (angleRad) * speed
		);

		transform.position = pos;
	}

	void CheckHit() {
		GameObject objectHit = CheckObjectHit(true);
		if (objectHit) {
			Damageable dm = objectHit.GetComponent<Damageable> ();
			if (dm) {
				dm.Damage (damage);
				HitInfo info = objectHit.GetComponent<HitInfo> ();
				if (info) {
					info.SetLastHitBy (owner);
				}
				Destroy (gameObject);
			}
		}
	}

	public static GameObject GetBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/RocketBullet");
	}

	public new static float GetRefireRate() {
		if (rocketInfo == null) {
			CreateRocketInfo();
		}
		return rocketInfo.refireRate;
	}

	public new static float GetBulletsPerShot() {
		if (rocketInfo == null) {
			CreateRocketInfo();
		}
		return rocketInfo.bulletsPerShot;
	}

	static void CreateRocketInfo() {
		Rocket temp = GetBullet ().GetComponent<Rocket>();
		rocketInfo = new ShootingInfo();
		rocketInfo.bulletsPerShot = temp.bulletsPerShot;
		rocketInfo.refireRate = temp.refireRate;
	}
}
