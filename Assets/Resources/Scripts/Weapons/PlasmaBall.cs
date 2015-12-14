using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlasmaBall : NetworkBehaviour {

	public float speed;

	public Vector3 pos;
	public Vector3 dest;

	[SyncVar] float radius;

	// Use this for initialization
	void Start () {
		pos = Vector3.zero;
		dest = pos;

		radius = 0;
	}
	
	// Update is called once per frame
	void Update () {

		Plasma plasma = gameObject.GetComponentInParent<Plasma> ();
		setRadius (plasma.getRadius ());

		float posX = pos.x;
		float posY = pos.y;
		float posZ = pos.z;

		if (Mathf.Abs (pos.x - dest.x) < speed) {
			posX = dest.x;
		} else {
			if (pos.x < dest.x) {
				posX += speed;
			} else if (pos.x > dest.x) {
				posX -= speed;		
			}
		}

		if (Mathf.Abs (pos.y - dest.y) < speed) {
			posY = dest.y;
		} else {
			if (pos.y < dest.y) {
				posY += speed;
			} else if (pos.y > dest.y) {
				posY -= speed;		
			}
		}

		if (Mathf.Abs (pos.z - dest.z) < speed) {
			posZ = dest.z;
		} else {
			if (pos.z < dest.z) {
				posZ += speed;
			} else if (pos.z > dest.z) {
				posZ -= speed;		
			}
		}

		pos = new Vector3 (posX, posY, posZ);
		transform.localPosition= pos;

		if ((pos.x == dest.x) && (pos.y == dest.y) && (pos.z == dest.z)) {
			newDest ();
		}
	}

	public void incRadius (float amount) {
		if (amount > 0) {
			radius += amount;
		}
	}

	public void decRadius (float amount) {
		if (amount > 0) {
			radius -= amount;
		}
	}

	public void setRadius (float newRadius) {
		if (newRadius > 0) {
			radius = newRadius;
		}
	}

	void newDest() {

		float newX, newY, newZ;

		//Choose a random angle in the circle
		float randAngle = (Random.Range (0.0f, 360.0f)) / Mathf.Rad2Deg;

		newX = Mathf.Cos (randAngle) * radius;
		newY = Mathf.Sin (randAngle) * radius;

		//Rotate the point around the axis in a random angle

		randAngle = (Random.Range (0.0f, 360.0f)) / Mathf.Rad2Deg;

		float distFromAxis = newX;

		newX = Mathf.Cos (randAngle) * distFromAxis;
		newZ = Mathf.Sin (randAngle) * distFromAxis;

		dest = new Vector3 (newX, newY, newZ);
	}
}
