using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Missile : Bullet {
	
	static ShootingInfo missileInfo = null;

	public float speedMultiplier;
	public float initialSpeed;
	float currentSpeed;
	
	// Use this for initialization
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
		
		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * currentSpeed,
			pos.y + Mathf.Cos (angleRad) * currentSpeed
		);

		transform.position = pos;
	}

	void checkHit() {
		Ship shipHit = checkShipHit ();
		if (shipHit != null) {
			//SoundPlayer.PlayClip(hitSound);
			shipHit.damage(damage);
			shipHit.setLastHitBy(owner.getOwner());
			Destroy (gameObject);
		}	
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (owner == null) {
			return;
		}
		
		GameObject objectHit = col.gameObject;
		
		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();
			if (shipHit != owner) {
				shipHit.damage(damage);
				shipHit.setLastHitBy(owner.getOwner());
				Destroy (gameObject);
			}
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
