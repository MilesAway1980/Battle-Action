﻿using UnityEngine;
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
		transform.Rotate( new Vector3 (0, 0, angleDeg));
	}

	void FixedUpdate() {

		distance = Vector2.Distance (originPos, transform.position);
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

		checkHit ();

		prevPos = pos;
		
		transform.position = new Vector2 (
			transform.position.x - Mathf.Sin (angleRad) * speed,
			transform.position.y + Mathf.Cos (angleRad) * speed
		);
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
