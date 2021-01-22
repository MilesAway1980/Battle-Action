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
	[SyncVar] bool warped;

	GameObject owner;

	void Start () {
		currentTime = 0;
		radius = transform.localScale.y;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(new Vector3 (0, 0, rotation));
		transform.localScale = Vector3.zero;
		warped = false;
	}
	
	void Update () {

		if (owner) {
			if ((Vector2)owner.transform.position == targetPos) {
				warped = true;
			}

			if (warped == false) {
				owner.transform.position = targetPos;
			}
		}

		if (warped) {
			currentTime += Time.deltaTime;
			if (currentTime > lingerTime) {
				Destroy (gameObject);
			} else {
				float currentRadius = radius * ((lingerTime - currentTime) / lingerTime);
				transform.localScale = new Vector3 (
					currentRadius,
					length,
					0
				);
			}
		}
	}

	public void Init(GameObject newOwner, Vector2 newPos, float newLength, float newRotation, Vector2 destination) {
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
		
		if (objectHit == owner) {
			return;
		}

		Damageable dm = objectHit.GetComponent<Damageable> ();
		if (dm) {
			dm.Damage (trailDamage);
			HitInfo info = objectHit.GetComponent<HitInfo> ();
			if (info) {
				info.SetLastHitBy (owner);
			}
		}		
	}
}
