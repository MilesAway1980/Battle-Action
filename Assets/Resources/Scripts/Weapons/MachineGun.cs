using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MachineGun : Bullet {

	//AudioSource audioSource;
	AudioClip hitSound;
	AudioClip shootSound;

	static ShootingInfo machinegunInfo;

	void Start () {

<<<<<<< HEAD
		travelDist = ArenaInfo.getArenaSize() * 1.25f;
=======
		//damage = 20;
		//speed = 1;

		travelDist = ArenaInfo.getArenaSize() * 2.5f;
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
		if (travelDist < ArenaInfo.getMinBulletTravelDist()) {
			travelDist = ArenaInfo.getMinBulletTravelDist();
		}

		//travelDist = 100;

		angleDeg += Random.Range (-2.0f, 2.0f);
		angleRad = angleDeg / Mathf.Rad2Deg;

		//Move the bullet out in front of the ship
		float forwardX = originPos.x - Mathf.Sin (angleRad) * 2;
		float forwardY = originPos.y + Mathf.Cos (angleRad) * 2;
		
		originPos = new Vector2 (forwardX, forwardY);
		
		transform.position = originPos;
		transform.Rotate( new Vector3 (0, 0, angleDeg));

		//setVelocity ();

		//hitSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Hit", typeof(AudioClip)));
		//shootSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Shoot1", typeof(AudioClip)));

		//SoundPlayer.PlayClip(shootSound);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//pos = transform.position;
		//float dist = Vector2.Distance (pos, originPos);
		//distance = dist;

		//if (Vector2.Distance (transform.position, originPos) > travelDist) {
			//Destroy (gameObject);
		//}

		/*if (rigidBody.velocity.magnitude < speed || transform.eulerAngles.z != angleRad) {
			setVelocity();
		}*/

		distance += speed;
		if (distance >= travelDist) {
			Destroy (gameObject);
		}

<<<<<<< HEAD
		checkHit ();
		prevPos = pos;

		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * speed,
			pos.y + Mathf.Cos (angleRad) * speed
		);
=======
		/*transform.position = new Vector2 (
			originPos.x - Mathf.Sin (angleRad) * distance,
			originPos.y + Mathf.Cos (angleRad) * distance
		);*/
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.

		transform.position = new Vector2 (
			transform.position.x - Mathf.Sin (angleRad) * speed,
			transform.position.y + Mathf.Cos (angleRad) * speed
		);

	}

<<<<<<< HEAD
	void checkHit() {
		Ship shipHit = checkShipHit ();
		if (shipHit != null) {
=======
	/*[Server]
	void setVelocity() {
		rigidBody.velocity = new Vector2 (
			-Mathf.Sin (angleRad) * speed,
			Mathf.Cos (angleRad) * speed
		);
	}*/

	//[Server]
	void OnCollisionEnter2D(Collision2D col) {

		if (owner == null) {
			return;
		}

		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
			//SoundPlayer.PlayClip(hitSound);
			shipHit.damage(damage);
			shipHit.setLastHitBy(owner.getOwner());
			Destroy (gameObject);
		}	
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Bullets/MachineGunBullet");
	}

	public static new float getRefireRate() {
		if (machinegunInfo == null) {
			createRocketInfo();
		}
		return machinegunInfo.refireRate;
	}
	
	public static new float getBulletsPerShot() {
		if (machinegunInfo == null) {
			createRocketInfo();
		}
		return machinegunInfo.bulletsPerShot;
	}
	
	static void createRocketInfo() {
		MachineGun temp = MachineGun.getBullet ().GetComponent<MachineGun>();
		machinegunInfo = new ShootingInfo();
		machinegunInfo.bulletsPerShot = temp.bulletsPerShot;
		machinegunInfo.refireRate = temp.refireRate;
	}
}
