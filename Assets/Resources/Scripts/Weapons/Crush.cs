using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Crush : Bullet {

	static ShootingInfo crushInfo;
	Ship target;
	[SyncVar] Vector3 size;

	public float crushSpeed;
	public float startingCrushLevel;
	public float minCrushLevel;

	[SyncVar] float crushLevel;
	[SyncVar] bool hasTarget;

	static ShotTimer shotTimer;

	// Use this for initialization
	void Start () {
		
		travelDist = ArenaInfo.getArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.getMinBulletTravelDist()) {
			travelDist = ArenaInfo.getMinBulletTravelDist();
		}

		angleRad = angleDeg / Mathf.Rad2Deg;

		//Move the bullet out in front of the ship

		originPos = new Vector2 (
			originPos.x - Mathf.Sin (angleRad) * 2,
			originPos.y + Mathf.Cos (angleRad) * 2
		);

		pos = originPos;

		transform.position = originPos;
		transform.Rotate( new Vector3 (0, 0, angleDeg));

		transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (hasTarget == false) {
			distance += speed;
			if (distance >= travelDist) {
				Destroy (gameObject);
			}

			prevPos = pos;

			pos = new Vector2 (
				pos.x - Mathf.Sin (angleRad) * speed,
				pos.y + Mathf.Cos (angleRad) * speed
			);
		}

		if (isServer) {
			if (target != null) {
				hasTarget = true;
				crushTarget ();
			} else {
				hasTarget = false;
				size = new Vector3 (0.2f, 0.2f, 0.2f);
				checkHit ();
			}
		}

		transform.position = pos;
		transform.localScale = size;
	}

	void checkHit() {
		Ship shipHit = checkShipHit (true);
		if (shipHit != null) {
			//SoundPlayer.PlayClip(hitSound);
			target = shipHit;
			crushLevel = startingCrushLevel;
		}	
	}

	void crushTarget() {
		pos = target.transform.position;
		size = new Vector3 (
			crushLevel,
			crushLevel,
			crushLevel
		);
		crushLevel -= crushSpeed;
		target.damage (damage);
		if (crushLevel < minCrushLevel) {
			Destroy (gameObject);
		}
	}

	public static ShotTimer getShotTimer() {
		if (shotTimer == null) {
			shotTimer = new ShotTimer ();
		}
		return shotTimer;
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Crush");
	}
	
	public static new float getRefireRate() {
		if (crushInfo == null) {
			createCrushInfo();
		}
		return crushInfo.refireRate;
	}
	
	public static new float getBulletsPerShot() {
		if (crushInfo == null) {
			createCrushInfo();
		}
		return crushInfo.bulletsPerShot;
	}
	
	static void createCrushInfo() {
		Crush temp = Crush.getBullet ().GetComponent<Crush>();
		crushInfo = new ShootingInfo();
		crushInfo.bulletsPerShot = temp.bulletsPerShot;
		crushInfo.refireRate = temp.refireRate;
	}
}
