using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Nuke : NetworkBehaviour {

	static float nukeRefireRate = -1;

	public float refireRate;
	public float damage;
	public float detonatorTime;
	public float maxRadius;
	public float expansionSpeed;
	public GameObject fireball;
	GameObject thisFireball;

	[SyncVar] public float currentTime;
	[SyncVar] public float currentRadius;
	[SyncVar] Vector2 pos;
	[SyncVar] Vector3 nukeSize;
	[SyncVar] Vector3 fireballSize;

	float angle;
	Ship owner;
	[SyncVar] bool exploded;

	// Use this for initialization
	void Start () {
		currentTime = 0;
		exploded = false;
		nukeSize = new Vector3 (0.3f, 0.3f, 0.3f);
		transform.localScale = nukeSize;
	}

	/*void Update() {
		print (nukeSize);
		print (fireballSize);
		print (thisFireball);
		transform.localScale = nukeSize;

	}*/

	void FixedUpdate () {

		if (exploded == false) {
			currentTime += Time.deltaTime;

			if (owner != null) {
				angle = owner.getAngle () / Mathf.Rad2Deg;
				pos = new Vector2 (
					owner.transform.position.x - Mathf.Sin (angle) * -2,
					owner.transform.position.y + Mathf.Cos (angle) * -2
				);
			}

			if (currentTime >= detonatorTime) {
				exploded = true;

				nukeSize = new Vector3(0, 0, 0);
				fireballSize = new Vector3(0, 0, 0);
				transform.localScale = nukeSize;

				thisFireball = (GameObject)Instantiate (fireball);
				thisFireball.transform.position = pos;
				thisFireball.transform.localScale = fireballSize;
			}

		} else {
			currentRadius += expansionSpeed;

			float cameraHeight = Mathf.Abs(Camera.main.transform.position.z);
			float explosionHeight = (currentRadius / maxRadius) * cameraHeight;

			fireballSize = new Vector3(currentRadius, currentRadius, explosionHeight);
			if (thisFireball != null) {
				thisFireball.transform.localScale = fireballSize;
			}
			checkDamage();

			if (currentRadius >= maxRadius) {
				Destroy (this.gameObject);
				Destroy (thisFireball);
			}
		}

		transform.position = pos;
	}

	[Server]
	void checkDamage() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
		if (players == null) {
			return;
		}

		for (int i = 0; i < players.Length; i++) {

			Ship whichShip = players[i].GetComponent<Ship>();
			if (whichShip == owner) {
				continue;
			}

			if (Vector2.Distance(
				pos,
				whichShip.transform.position
			) <= (currentRadius / 2) ) {
				whichShip.damage(damage);
			}
		}
	}

	public void init(Ship newOwner) {
		owner = newOwner;
	}

	public static GameObject getBomb() {
		return (GameObject)Resources.Load ("Prefabs/3D Weapons/Nuke");
	}
	
	public static float getRefireRate() {
		if (nukeRefireRate == -1) {
			Nuke nuke = Nuke.getBomb().GetComponent<Nuke>();
			nukeRefireRate = nuke.refireRate;
		}

		return nukeRefireRate;
	}
}
