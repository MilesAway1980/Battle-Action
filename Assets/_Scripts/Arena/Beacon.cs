using UnityEngine;
using Mirror;
using System.Collections;

public class Beacon : NetworkBehaviour
{

	public static ObjectList beaconList = new ObjectList();

	void Awake()
	{
		beaconList.AddObject(gameObject);
	}

	void OnDestroy()
	{

		beaconList.RemoveObject(gameObject);
	}
}
