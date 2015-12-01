using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnBeacons : NetworkBehaviour {

	public GameObject beacons;

	GameObject allBeacons;

	// Use this for initialization

	void Start () {


	}

	void Update() {
		if (allBeacons == null) {
			populateBeacons ();
		}
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
