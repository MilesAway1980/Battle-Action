using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour
{

	public Vector3 rotateSpeed;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		this.transform.Rotate(
			rotateSpeed
		);
	}
}
