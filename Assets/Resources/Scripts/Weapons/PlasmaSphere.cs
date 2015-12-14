using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlasmaSphere : NetworkBehaviour {

	[SyncVar] float radius;

	// Use this for initialization
	void Start () {
		radius = 0;
	}

	// Update is called once per frame
	void Update () {

		Plasma plasma = gameObject.GetComponentInParent<Plasma> ();
		setRadius (plasma.getRadius ());

		transform.localScale = new Vector3 (radius, radius, radius);
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
}
