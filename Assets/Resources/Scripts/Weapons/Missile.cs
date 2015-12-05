﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Missile : Bullet {
	
	static ShootingInfo missileInfo = null;
	
	// Use this for initialization
	void Start () {	

		travelDist = ArenaInfo.getArenaSize () * 0.5f;
		if (travelDist < ArenaInfo.getMinBulletTravelDist ()) {
			travelDist = ArenaInfo.getMinBulletTravelDist ();
		}
						
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
		
		transform.position = new Vector2 (
			transform.position.x - Mathf.Sin (angleRad) * speed,
			transform.position.y + Mathf.Cos (angleRad) * speed
			);
	}
	
	[Server]
	void OnCollisionEnter2D(Collision2D col) {
		
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
		return (GameObject)Resources.Load ("Prefabs/Bullets/Missile");
	}
	
	public static float getRefireRate() {
		if (missileInfo == null) {
			createMissileInfo();
		}
		return missileInfo.refireRate;
	}
	
	public static float getBulletsPerShot() {
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
