using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Bullet : NetworkBehaviour {

	protected Ship owner;
	protected Vector2 originPos;
	protected Vector2 pos;
	protected float angleRad;
	protected float angleDeg;
	protected int type;
	protected float travelDist;

	protected float speed;
	protected float distance;

	protected Rigidbody2D rigidBody;
	protected CircleCollider2D circleCollider;

	protected float damage;

	void Awake() {		
		rigidBody = gameObject.GetComponent<Rigidbody2D> ();
		if (rigidBody == null) {
			rigidBody = gameObject.AddComponent<Rigidbody2D>();
		}

		circleCollider = gameObject.GetComponent<CircleCollider2D> ();
		if (circleCollider == null) {
			circleCollider = gameObject.AddComponent<CircleCollider2D> ();
		}
	}

	void Update() {

	}

	[Server]
	public void init(Ship newOwner, Vector2 startPos, float newAngle, int newType) {

		angleRad = newAngle / Mathf.Rad2Deg;

		//Move the bullet out in front of the ship
		float forwardX = startPos.x - Mathf.Sin (angleRad) * 2;
		float forwardY = startPos.y + Mathf.Cos (angleRad) * 2;

		startPos = new Vector2 (forwardX, forwardY);

		owner = newOwner;
		originPos = startPos;
		pos = startPos;
		angleDeg = newAngle;
		type = newType;

		transform.position = startPos;
		transform.Rotate( new Vector3 (0, 0, newAngle));
	}


}
