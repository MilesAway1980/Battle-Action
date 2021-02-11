using UnityEngine;
using System.Collections;

public class PlasmaSphere : MonoBehaviour {

	float radius;

	// Use this for initialization
	void Start () {
		radius = 0;
	}

	// Update is called once per frame
	void Update () {

		Plasma plasma = gameObject.GetComponentInParent<Plasma> ();
		SetRadius (plasma.GetRadius ());

		transform.localScale = new Vector3 (radius, radius, radius);
	}

	public void IncreaseRadius (float amount) {
		if (amount > 0) {
			radius += amount;
		}
	}

	public void DecreaseRadius (float amount) {
		if (amount > 0) {
			radius -= amount;
		}
	}

	public void SetRadius (float newRadius) {
		if (newRadius > 0) {
			radius = newRadius;
		}
	}
}
