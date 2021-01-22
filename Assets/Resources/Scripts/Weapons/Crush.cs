using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Crush : Bullet {

	static ShootingInfo crushInfo;
	GameObject target;
	[SyncVar] Vector3 size;

	public float crushSpeed;
	public float startingCrushLevel;
	public float minCrushLevel;

	[SyncVar] float crushLevel;
	[SyncVar] bool hasTarget;

	// Use this for initialization
	void Start () {
		
		travelDist = ArenaInfo.GetArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist()) {
			travelDist = ArenaInfo.GetMinBulletTravelDist();
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
				CrushTarget ();
			} else {
				hasTarget = false;
				size = new Vector3 (0.2f, 0.2f, 0.2f);
				CheckHit ();
			}
		}

		transform.position = pos;
		transform.localScale = size;
	}

	void CheckHit() {
		GameObject objectHit = CheckObjectHit(true);
		if (objectHit != null) {
			//SoundPlayer.PlayClip(hitSound);
			target = objectHit;
			crushLevel = startingCrushLevel;
		}	
	}

	void CrushTarget() {
		pos = target.transform.position;
		size = new Vector3 (
			crushLevel,
			crushLevel,
			crushLevel
		);
		crushLevel -= crushSpeed;
		Damageable dm = target.GetComponent<Damageable> ();
		if (dm) {
			dm.Damage (damage);
		}

		if (crushLevel < minCrushLevel) {
			Destroy (gameObject);
		}
	}

	public static GameObject GetBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Crush");
	}
	
	public static new float GetRefireRate() {
		if (crushInfo == null) {
			CreateCrushInfo();
		}
		return crushInfo.refireRate;
	}
	
	public static new float GetBulletsPerShot() {
		if (crushInfo == null) {
			CreateCrushInfo();
		}
		return crushInfo.bulletsPerShot;
	}
	
	static void CreateCrushInfo() {
		Crush temp = GetBullet ().GetComponent<Crush>();
		crushInfo = new ShootingInfo();
		crushInfo.bulletsPerShot = temp.bulletsPerShot;
		crushInfo.refireRate = temp.refireRate;
	}
}
