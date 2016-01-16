using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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

	float angle;

	GameObject owner;


	[SyncVar] bool exploded;

	// Use this for initialization
	void Start () {
		currentTime = 0;
		exploded = false;
		nukeSize = new Vector3 (0.3f, 0.3f, 0.3f);
		transform.localScale = Vector3.zero;
	}

	void FixedUpdate () {

		if (exploded == false) {
			currentTime += Time.deltaTime;

			if (owner) {
				angle = owner.transform.eulerAngles.z / Mathf.Rad2Deg;
				pos = new Vector2 (
					owner.transform.position.x - Mathf.Sin (angle) * -2,
					owner.transform.position.y + Mathf.Cos (angle) * -2
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


	void checkDamage() {
		if (!isServer) {
			return;
		}

		List<GameObject> damageableObjects = Damageable.damageableList.getObjectList();
		if (damageableObjects.Count == 0) {
			return;
		}

		for (int i = 0; i < damageableObjects.Count; i++) {
			GameObject target = damageableObjects [i].gameObject;

			if (target) {
				if (target == owner) {
					continue;
				}
			}

			float targetDist = Vector2.Distance (
				pos,
				target.transform.position
			);

			if (targetDist <= (currentRadius / 2.0f)) {
				damageableObjects [i].GetComponent<Damageable>().damage (damage);
			}
		}
	}

	public void init(GameObject newOwner) {
		owner = newOwner;
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
