using UnityEngine;
using System.Collections;

public class RotateHead : MonoBehaviour {

	public GameObject head;
	public float turnRate;
	Rigidbody2D headRB;
	Transform headTransform;
	float angle;


	// Use this for initialization
	void Start () {
		headRB = head.GetComponent<Rigidbody2D> ();
		if (headRB == null) {
			headRB = head.AddComponent<Rigidbody2D>();
		}

		headTransform = head.transform;
		angle = Random.Range (0, 360);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		angle += turnRate;
		//headRB.rotation = new Vector3 (0, 0, angle);
		headTransform.Rotate (new Vector3 (0, 0, turnRate));
	}
}
