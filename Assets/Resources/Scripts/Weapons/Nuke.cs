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
	public GameObject thisFireball;

	[SyncVar] public float currentTime;
	[SyncVar] public float currentRadius;
	[SyncVar] Vector2 pos;
	[SyncVar] public Vector3 nukeSize;
	[SyncVar] public Vector3 fireballSize;
	static float lastShot;

	float angle;
	Player owner;
	Ship ownerShip;
	[SyncVar] bool exploded;

	// Use this for initialization
	void Start () {
		currentTime = 0;
		exploded = false;
		nukeSize = new Vector3 (0.3f, 0.3f, 0.3f);
		transform.localScale = Vector3.zero;
	}

	void FixedUpdate () {

		//print (this.transform.parent);
		//print (this.transform.position);

		if (owner != null) {
			if (ownerShip == null) {			
				ownerShip = owner.getShip ();
				if (ownerShip == null) {
					return;
				}
			}
		}

		if (exploded == false) {
			currentTime += Time.deltaTime;

			if (ownerShip != null) {
				angle = ownerShip.getAngle () / Mathf.Rad2Deg;
				pos = new Vector2 (
					ownerShip.transform.position.x - Mathf.Sin (angle) * -2,
					ownerShip.transform.position.y + Mathf.Cos (angle) * -2
				);
			}

			if (currentTime >= detonatorTime) {
				exploded = true;

				nukeSize = new Vector3 (0, 0, 0);
				fireballSize = new Vector3 (0, 0, 0);

				thisFireball = (GameObject)Instantiate (fireball);
				thisFireball.transform.position = pos;
				thisFireball.transform.localScale = fireballSize;
			} 

			transform.localScale = nukeSize;

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
				
				if (thisFireball != null) {
					Destroy (thisFireball);
				}

				//Only destroy the Nuke object if the fireball is gone
				//Otherwise, it risks the chance of leaving a permanent fireball
				if (thisFireball == null) {
					Destroy (this.gameObject);
				}
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
			if (whichShip == ownerShip) {
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

	public void init(Player newOwner) {
		owner = newOwner;
	}

	public static float getLastShot() {
		return lastShot;
	}

	public static void updateLastShot() {
		lastShot = Time.fixedTime;
	}

	public static GameObject getBomb() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Nuke");
	}
	
	public static float getRefireRate() {
		if (nukeRefireRate == -1) {
			Nuke nuke = Nuke.getBomb().GetComponent<Nuke>();
			nukeRefireRate = nuke.refireRate;
		}

		return nukeRefireRate;
	}
}
