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
<<<<<<< HEAD
<<<<<<< HEAD
		travelDist = ArenaInfo.getArenaSize() * 1.25f;
=======
		//damage = 20;
		//speed = 1;

		travelDist = ArenaInfo.getArenaSize() * 2.5f;
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
=======
=======
		//damage = 20;
		//speed = 1;

>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
		travelDist = ArenaInfo.getArenaSize() * 2.5f;
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
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
<<<<<<< HEAD
<<<<<<< HEAD
		checkHit ();
		prevPos = pos;

=======
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
		pos = new Vector2 (
			pos.x - Mathf.Sin (angleRad) * speed,
			pos.y + Mathf.Cos (angleRad) * speed
		);
=======
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
		/*transform.position = new Vector2 (
			originPos.x - Mathf.Sin (angleRad) * distance,
			originPos.y + Mathf.Cos (angleRad) * distance
		);*/
<<<<<<< HEAD
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.

		transform.position = new Vector2 (
			transform.position.x - Mathf.Sin (angleRad) * speed,
			transform.position.y + Mathf.Cos (angleRad) * speed
		);


	}

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
	void checkHit() {
		Ship shipHit = checkShipHit ();
		if (shipHit != null) {
=======
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
	/*[Server]
	void setVelocity() {
		rigidBody.velocity = new Vector2 (
			-Mathf.Sin (angleRad) * speed,
			Mathf.Cos (angleRad) * speed
		);
	}*/

	//[Server]
<<<<<<< HEAD
=======
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
=======
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
	void OnCollisionEnter2D(Collision2D col) {

		if (owner == null) {
			return;
		}

		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {
<<<<<<< HEAD
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
=======
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
			//SoundPlayer.PlayClip(hitSound);

			Ship shipHit = objectHit.GetComponent<Ship>();
			if (shipHit != owner) {
				shipHit.damage(damage);
				shipHit.setLastHitBy(owner.getOwner());
				Destroy (gameObject);
			}
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
