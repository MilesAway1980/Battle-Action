using UnityEngine;
using System.Collections;

public class MachineGun : Bullet {

	//AudioSource audioSource;
	AudioClip hitSound;
	AudioClip shootSound;

	// Use this for initialization
	void Start () {
		this.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);

		speed = 50;
		travelDist = ArenaInfo.getArenaSize() * 2.5f;

		angleDeg += Random.Range (-2.0f, 2.0f);
		angleRad = angleDeg / Mathf.Rad2Deg;

		setVelocity ();



		hitSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Hit", typeof(AudioClip)));
		shootSound = Instantiate ((AudioClip)Resources.Load ("Audio/Sound/Shoot1", typeof(AudioClip)));

		//SoundPlayer.PlayClip (shootSound);

		//audioSource = SoundPlayer.getAudioSource ();
		//audioSource = gameObject.AddComponent<AudioSource> ();
		//audioSource.clip = shootSound;
		//audioSource.PlayOneShot (shootSound);

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		pos = transform.position;
		float dist = Vector2.Distance (pos, originPos);
		distance = dist;

		if (dist > travelDist) {
			Destroy (gameObject);
		}

		if (rigidBody.velocity.magnitude < speed || transform.eulerAngles.z != angleRad) {
			setVelocity();
		}
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Bullet");
	}

	void OnCollisionEnter2D(Collision2D col) {

		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {

			//audioSource.clip = hitSound;
			//audioSource.PlayOneShot (hitSound);
			SoundPlayer.PlayClip(hitSound);

			Ship shipHit = objectHit.GetComponent<Ship>();
			shipHit.damage(1);

			Destroy (gameObject);
		}
	}

	void setVelocity() {
		rigidBody.velocity = new Vector2 (
			-Mathf.Sin (angleRad) * speed,
			Mathf.Cos (angleRad) * speed
			);
	}
}
