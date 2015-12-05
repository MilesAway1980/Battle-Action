using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MachineGun : Bullet {

	//AudioSource audioSource;
	AudioClip hitSound;
	AudioClip shootSound;

	static ShootingInfo machinegunInfo;

	void Start () {

		//damage = 20;
		//speed = 1;

		travelDist = ArenaInfo.getArenaSize() * 2.5f;
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

		transform.position = new Vector2 (
			originPos.x - Mathf.Sin (angleRad) * distance,
			originPos.y + Mathf.Cos (angleRad) * distance
		);


	}

	/*[Server]
	void setVelocity() {
		rigidBody.velocity = new Vector2 (
			-Mathf.Sin (angleRad) * speed,
			Mathf.Cos (angleRad) * speed
		);
	}*/

	[Server]
	void OnCollisionEnter2D(Collision2D col) {

		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {

			//SoundPlayer.PlayClip(hitSound);

			Ship shipHit = objectHit.GetComponent<Ship>();
			if (shipHit != owner) {
				shipHit.damage(damage);
				//shipHit.setLastHitBy(owner);
				shipHit.setLastHitBy(owner.getOwner());
				Destroy (gameObject);
			}


		}
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Bullets/MachineGunBullet");
	}

	public static float getRefireRate() {
		if (machinegunInfo == null) {
			createRocketInfo();
		}
		return machinegunInfo.refireRate;
	}
	
	public static float getBulletsPerShot() {
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
