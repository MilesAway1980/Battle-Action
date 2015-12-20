using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MachineGun : Bullet {

	//AudioSource audioSource;
	AudioClip hitSound;
	AudioClip shootSound;

	static ShootingInfo machinegunInfo;

	void Start () {

		travelDist = ArenaInfo.getArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.getMinBulletTravelDist()) {
			travelDist = ArenaInfo.getMinBulletTravelDist();
		}

		angleDeg += Random.Range (-2.0f, 2.0f);
		angleRad = angleDeg / Mathf.Rad2Deg;

		//Move the bullet out in front of the ship

		originPos = new Vector2 (
			originPos.x - Mathf.Sin (angleRad) * 2,
			originPos.y + Mathf.Cos (angleRad) * 2
		);
		
		pos = originPos;
		
		transform.position = originPos;
		transform.Rotate( new Vector3 (0, 0, angleDeg));

		//hitSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Hit", typeof(AudioClip)));
		//shootSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Shoot1", typeof(AudioClip)));

		//SoundPlayer.PlayClip(shootSound);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		distance += speed;
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
			shipHit.setLastHitBy(owner.getPlayerNum());
			Destroy (gameObject);
		}	
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/MachineGunBullet");
	}

	public static new float getRefireRate() {
		if (machinegunInfo == null) {
			createMachineGunInfo();
		}
		return machinegunInfo.refireRate;
	}
	
	public static new float getBulletsPerShot() {
		if (machinegunInfo == null) {
			createMachineGunInfo();
		}
		return machinegunInfo.bulletsPerShot;
	}
	
	static void createMachineGunInfo() {
		MachineGun temp = MachineGun.getBullet ().GetComponent<MachineGun>();
		machinegunInfo = new ShootingInfo();
		machinegunInfo.bulletsPerShot = temp.bulletsPerShot;
		machinegunInfo.refireRate = temp.refireRate;
	}
}
