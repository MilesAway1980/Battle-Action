using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnBeacons : NetworkBehaviour {

	public GameObject beacons;

	GameObject allBeacons;

	static bool done;

	// Use this for initialization

	void Start () {

	}

	void Update() {
		if (done == false) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
			if (players.Length > 0) {
				done = true;
				PopulateBeacons();
			}
		}


		/*if (allBeacons == null && done == false) {
			populateBeacons ();
			done = true;
		}*/
	}

	[Server]
	void PopulateBeacons() {

		allBeacons = new GameObject ();
		allBeacons.name = "Beacons";

		for (int i = 0; i < ArenaInfo.GetNumBeacons(); i++) {
			GameObject newBeacon = Instantiate (
				beacons,
				ArenaInfo.GetRandomArenaLocation(),				
				Quaternion.identity
			);
		
			newBeacon.transform.parent = allBeacons.transform;
		
			NetworkServer.Spawn (newBeacon);

		}
	}
}
