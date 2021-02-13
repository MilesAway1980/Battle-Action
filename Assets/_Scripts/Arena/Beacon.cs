using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

public class Beacon : NetworkBehaviour
{

	public static ObjectList beaconList = new ObjectList();
	public bool active;
	public List<Guid> guidsBeaconWorksFor;

	void Awake()
	{
		beaconList.AddObject(gameObject);
	}

	void OnDestroy()
	{

		beaconList.RemoveObject(gameObject);
	}
}
