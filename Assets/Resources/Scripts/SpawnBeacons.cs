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
				populateBeacons();
			}
		}


		/*if (allBeacons == null && done == false) {
			populateBeacons ();
			done = true;
		}*/
	}

	[Server]
	void populateBeacons() {

		allBeacons = new GameObject ();
		allBeacons.name = "Beacons";

		for (int i = 0; i < ArenaInfo.getNumBeacons(); i++) {
			GameObject newBeacon = (GameObject)Instantiate (
				beacons, 
				new Vector2 (
					Random.Range (-ArenaInfo.getArenaSize (), ArenaInfo.getArenaSize ()), 
					Random.Range (-ArenaInfo.getArenaSize (), ArenaInfo.getArenaSize ())
				),
				Quaternion.identity
			);
		
			newBeacon.transform.parent = allBeacons.transform;
		
			NetworkServer.Spawn (newBeacon);

		}
	}
}
