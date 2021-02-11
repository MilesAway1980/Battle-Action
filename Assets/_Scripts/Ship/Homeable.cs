using UnityEngine;
using System.Collections;

public class Homeable : MonoBehaviour
{

	bool isHomeable;

	// Use this for initialization
	void Start()
	{
		isHomeable = true;
	}

	public bool GetIsHomeable()
	{
		return isHomeable;
	}

	public void SetIsHomeable(bool value)
	{
		isHomeable = value;
	}
}
