using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Beacon : NetworkBehaviour {

	static ObjectList beacons;

	void Awake() {

		if (beacons == null) {
			beacons = new ObjectList();
		}

		beacons.addObject (this.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static ObjectInfo getNearestBeacon(GameObject which) {
		if (beacons != null) {
			return beacons.getClosest (which);
		} else {
			return new ObjectInfo();
		}
	}
}
