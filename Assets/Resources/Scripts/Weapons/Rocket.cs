using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Rocket : Bullet {

	public int range;
	static ShootingInfo rocketInfo = null;

	// Use this for initialization
	void Start () {
		//damage = 15;
		//speed = 0.05f;

		/*travelDist = ArenaInfo.getArenaSize () * 0.5f;
		if (travelDist < ArenaInfo.getMinBulletTravelDist ()) {
			travelDist = ArenaInfo.getMinBulletTravelDist ();
		}*/

		travelDist = range;

		float forwardX = originPos.x - Mathf.Sin (angleRad) * 2;
		float forwardY = originPos.y + Mathf.Cos (angleRad) * 2;
		
		originPos = new Vector2 (forwardX, forwardY);
		
		transform.position = originPos;
		transform.Rotate( new Vector3 (0, 0, angleDeg));
	}

	void FixedUpdate() {
		//distance += speed;
		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}
		
		transform.position = new Vector2 (
			transform.position.x - Mathf.Sin (angleRad) * speed,
			transform.position.y + Mathf.Cos (angleRad) * speed
		);
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
		return (GameObject)Resources.Load ("Prefabs/Bullets/RocketBullet");
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

	static void createRocketInfo() {
		Rocket temp = Rocket.getBullet ().GetComponent<Rocket>();
		rocketInfo = new ShootingInfo();
		rocketInfo.bulletsPerShot = temp.bulletsPerShot;
		rocketInfo.refireRate = temp.refireRate;
	}
}
