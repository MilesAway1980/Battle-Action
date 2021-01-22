using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Beacon : NetworkBehaviour {

	public static ObjectList beaconList;

	void Awake() {

		if (beaconList == null) {
			beaconList = new ObjectList();
		}

		beaconList.AddObject(gameObject);
	}
}
