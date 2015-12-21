using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Rocket : Bullet {

	public int range;
	static ShootingInfo rocketInfo = null;

	static ShotTimer shotTimer;

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

		checkHit ();

		prevPos = pos;
		
		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * speed,
			pos.y + Mathf.Cos (angleRad) * speed
		);

		transform.position = pos;
	}

	void checkHit() {
		Ship shipHit = checkShipHit (true);
		if (shipHit != null) {
			//SoundPlayer.PlayClip(hitSound);
			shipHit.damage(damage);
			shipHit.setLastHitBy (owner.getPlayerNum ());
			Destroy (gameObject);
		}	
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/RocketBullet");
	}

	public new static float getRefireRate() {
		if (rocketInfo == null) {
			createRocketInfo();
		}
		return rocketInfo.refireRate;
	}

	public new static float getBulletsPerShot() {
		if (rocketInfo == null) {
			createRocketInfo();
		}
		return rocketInfo.bulletsPerShot;
	}

	public static ShotTimer getShotTimer() {
		if (shotTimer == null) {
			shotTimer = new ShotTimer ();
		}
		return shotTimer;
	}

	static void createRocketInfo() {
		Rocket temp = Rocket.getBullet ().GetComponent<Rocket>();
		rocketInfo = new ShootingInfo();
		rocketInfo.bulletsPerShot = temp.bulletsPerShot;
		rocketInfo.refireRate = temp.refireRate;
	}
}
