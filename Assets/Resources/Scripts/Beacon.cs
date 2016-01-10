using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Beacon : NetworkBehaviour {

	public static ObjectList beacons;

	void Awake() {

		if (beacons == null) {
			beacons = new ObjectList();
		}

		beacons.addObject (this.gameObject);
	}
}
