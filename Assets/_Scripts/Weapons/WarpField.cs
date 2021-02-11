using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class WarpField : MonoBehaviour
{

	public float lingerTime;	
	public float startingRadius;

	float currentTime;
	float length;
	bool warped;

	void Start()
	{
		warped = false;	
	}

	void FixedUpdate()
	{
		if (warped == false)
        {
			return;
        }
		
		currentTime += Time.deltaTime;
		if (currentTime > lingerTime)
		{
			Destroy(gameObject);
		}
		else
		{
			float currentRadius = startingRadius * ((lingerTime - currentTime) / lingerTime);
			transform.localScale = new Vector3(
				currentRadius,
				length,
				0
			);
		}

	}

	[Server]
	public void Init(float rotation, Vector2 warpCenter, float length)
	{
		transform.position = warpCenter;
		transform.rotation = Quaternion.Euler(0, 0, rotation);

		this.length = length;

		transform.localScale = new Vector3(5, length, 5);

		warped = true;
	}	
}
