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
	[SyncVar] Vector2 targetPos;	//Where the ship is warping to

	Player owner;
	Ship ownerShip;

	[SyncVar] bool warped;

	// Use this for initialization
	void Start () {
		currentTime = 0;
		radius = transform.localScale.y;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(new Vector3 (0, 0, rotation));
		transform.localScale = Vector3.zero;
		warped = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (owner != null) {
			if (ownerShip == null) {			
				ownerShip = owner.getShip ();
				if (ownerShip == null) {
					return;
				}
			}
		}

		if (ownerShip != null) {
			if (warped == false) {
				ownerShip.transform.position = targetPos;
			}
			warped = true;
		}

		if (warped == false) {
			return;
		}

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

	public void init(Player newOwner, Vector2 newPos, float newLength, float newRotation, Vector2 destination) {
		owner = newOwner;
		pos = newPos;
		length = newLength;
		rotation = newRotation;
		targetPos = destination;
	}

	void OnTriggerStay2D(Collider2D col) {
		if (!isServer) {
			return;
		}
		
		GameObject objectHit = col.gameObject;
		
		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();
			if (shipHit == ownerShip) {
				return;
			}

			shipHit.damage(trailDamage);
			shipHit.setLastHitBy (owner.getPlayerNum ());
		}
		
	}
}
