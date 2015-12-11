using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WarpField : NetworkBehaviour {

	public float lingerTime;
	public float trailDamage;

	[SyncVar] float currentTime;
	[SyncVar] float radius;
	[SyncVar] float length;
	[SyncVar] float rotation;
	[SyncVar] Vector2 pos;


	Ship owner;

	// Use this for initialization
	void Start () {
		currentTime = 0;
		radius = transform.localScale.y;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(new Vector3 (0, 0, rotation));
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if (currentTime > lingerTime) {
			Destroy (gameObject);
		} else {

			float currentRadius = radius * ((lingerTime - currentTime) / lingerTime);
			transform.localScale = new Vector3(
				currentRadius,
				length,
				0
			);
		}
	}

	public void init(Ship newOwner, Vector2 newPos, float newLength, float newRotation) {
		owner = newOwner;
		pos = newPos;
		length = newLength;
		rotation = newRotation;
	}

	void OnTriggerStay2D(Collider2D col) {
		if (!isServer) {
			return;
		}
		
		GameObject objectHit = col.gameObject;
		
		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();
			if (shipHit == owner) {
				return;
			}

			shipHit.damage(trailDamage);
			shipHit.setLastHitBy(owner.getOwner());
		}
		
	}
}
