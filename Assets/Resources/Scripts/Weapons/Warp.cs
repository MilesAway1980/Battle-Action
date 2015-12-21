using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Warp : NetworkBehaviour {

	public float warpTimer;

	[Range (0, 100)]
	public float minPercentageOfArenaToWarp;
	[Range (0, 100)]
	public float maxPercentageOfArenaToWarp;

	Player owner;
	Ship ownerShip;

	[SyncVar] float currentTime;

	public float minimumWarpDistance;
	public float refireRate;
	static float warpRefireRate = -1;

	// Use this for initialization
	void Start () {
		currentTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (owner != null) {
			if (ownerShip == null) {			
				ownerShip = owner.getShip ();
			}
		}

		currentTime += Time.deltaTime;
		if (currentTime > warpTimer) {
			warpShip();
		}
	}

	[Server]
	void warpShip() {
		
		float min = (ArenaInfo.getArenaSize () * (minPercentageOfArenaToWarp / 100.0f));
		float max = (ArenaInfo.getArenaSize () * (maxPercentageOfArenaToWarp / 100.0f));		
		float distance = Random.Range (min, max);

		if (distance < minimumWarpDistance) {
			distance = minimumWarpDistance;
		}

		float angleRad = ownerShip.getAngle () / Mathf.Rad2Deg;

		Vector2 halfWay = new Vector2 (
			ownerShip.transform.position.x - Mathf.Sin (angleRad) * (distance * 0.5f),
			ownerShip.transform.position.y + Mathf.Cos (angleRad) * (distance * 0.5f)
		);

		Vector2 destination = new Vector3 (
			ownerShip.transform.position.x - Mathf.Sin (angleRad) * distance,
			ownerShip.transform.position.y + Mathf.Cos (angleRad) * distance
		);

		GameObject warpField = (GameObject)Instantiate (getWarpField (), halfWay, Quaternion.identity);

		WarpField wf = warpField.GetComponent<WarpField> ();
		wf.init (owner, halfWay, distance * 0.5f, ownerShip.getAngle (), destination);

		NetworkServer.Spawn (warpField);
		
		Destroy (gameObject);
	}

	public static GameObject getWarp() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Warp");
	}

	public static GameObject getWarpField() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Warp Field");
	}

	public static float getRefireRate() {
		if (warpRefireRate == -1) {
			warpRefireRate = getWarp ().GetComponent<Warp> ().refireRate;
		}
		return warpRefireRate;
	}

	public void setOwner(Player newOwner) {
		owner = newOwner;
	}
}
