using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Warp : NetworkBehaviour {

	public float warpTimer;

	[Range (0, 100)]
	public float minPercentageOfArenaToWarp;
	[Range (0, 100)]
	public float maxPercentageOfArenaToWarp;
	Ship owner;

	[SyncVar] public float currentTime;



	// Use this for initialization
	void Start () {
		currentTime = 0;

		if (minPercentageOfArenaToWarp == 0 && maxPercentageOfArenaToWarp == 0) {
			minPercentageOfArenaToWarp = getWarp ().GetComponent<Warp>().minPercentageOfArenaToWarp;
			maxPercentageOfArenaToWarp = getWarp ().GetComponent<Warp>().maxPercentageOfArenaToWarp;
			warpTimer = getWarp ().GetComponent<Warp>().warpTimer;
		}
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if (currentTime > warpTimer) {
			warpShip();
		}
	}

	[Server]
	void warpShip() {
		float maxWarpDist = (ArenaInfo.getArenaSize () * (maxPercentageOfArenaToWarp / 100.0f));
		float minWarpDist = (ArenaInfo.getArenaSize () * (minPercentageOfArenaToWarp / 100.0f));
		
		float distance = Random.Range (minWarpDist, maxWarpDist);
		float angleRad = owner.getAngle () / Mathf.Rad2Deg;

		Vector2 halfWay = new Vector2 (
			owner.transform.position.x - Mathf.Sin (angleRad) * (distance * 0.5f),
			owner.transform.position.y + Mathf.Cos (angleRad) * (distance * 0.5f)
			);

		owner.transform.position = new Vector3 (
			owner.transform.position.x - Mathf.Sin (angleRad) * distance,
			owner.transform.position.y + Mathf.Cos (angleRad) * distance
		);



		GameObject warpField = (GameObject)Instantiate (getWarpField ());

		WarpField wf = warpField.GetComponent<WarpField> ();
		wf.init (owner, halfWay, distance * 0.5f, owner.getAngle ());


		NetworkServer.Spawn (warpField);
		
		Destroy (gameObject);
	}

	public static GameObject getWarp() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Warp");
	}

	public static GameObject getWarpField() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Warp Field");
	}

	public void setOwner(Ship newOwner) {
		owner = newOwner;
	}
}
