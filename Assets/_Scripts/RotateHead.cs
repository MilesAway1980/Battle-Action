using UnityEngine;
using System.Collections;

public class RotateHead : MonoBehaviour
{

	public GameObject head;
	public float turnRateX;
	public float turnRateY;
	public float turnRateZ;
	Rigidbody2D headRB;
	Transform headTransform;

	void Start()
	{
		headRB = head.GetComponent<Rigidbody2D>();
		if (headRB == null)
		{
			headRB = head.AddComponent<Rigidbody2D>();
		}

		headTransform = head.transform;

		float randomX = turnRateX != 0 ? Random.Range(0, 360) : 0;
		float randomY = turnRateY != 0 ? Random.Range(0, 360) : 0;
		float randomZ = turnRateZ != 0 ? Random.Range(0, 360) : 0;

		headTransform.Rotate(new Vector3(randomX, randomY, randomZ));
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		headTransform.Rotate(new Vector3(turnRateX * Time.deltaTime, turnRateY * Time.deltaTime, turnRateZ * Time.deltaTime));
	}
}
