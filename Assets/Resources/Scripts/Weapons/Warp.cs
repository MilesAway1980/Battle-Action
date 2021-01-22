using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Warp : NetworkBehaviour {

	public float warpTimer;

	[Range (0, 100)]
	public float minPercentageOfArenaToWarp;
	[Range (0, 100)]
	public float maxPercentageOfArenaToWarp;

	GameObject owner;

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
		
		currentTime += Time.deltaTime;
		if (currentTime > warpTimer) {
			warpShip();
		}
	}

	[Server]
	void warpShip() {
		
		float min = (ArenaInfo.GetArenaSize () * (minPercentageOfArenaToWarp / 100.0f));
		float max = (ArenaInfo.GetArenaSize () * (maxPercentageOfArenaToWarp / 100.0f));		
		float distance = Random.Range (min, max);

		if (distance < minimumWarpDistance) {
			distance = minimumWarpDistance;
		}

		float angleRad = owner.transform.eulerAngles.z / Mathf.Rad2Deg;

		Vector2 halfWay = new Vector2 (
			owner.transform.position.x - Mathf.Sin (angleRad) * (distance * 0.5f),
			owner.transform.position.y + Mathf.Cos (angleRad) * (distance * 0.5f)
		);

		Vector2 destination = new Vector3 (
			owner.transform.position.x - Mathf.Sin (angleRad) * distance,
			owner.transform.position.y + Mathf.Cos (angleRad) * distance
		);

		GameObject warpField = Instantiate (GetWarpField (), halfWay, Quaternion.identity);

		WarpField wf = warpField.GetComponent<WarpField> ();
		wf.Init (owner, halfWay, distance * 0.5f, owner.transform.eulerAngles.z, destination);

		NetworkServer.Spawn (warpField);
		
		Destroy (gameObject);
	}

	public static GameObject GetWarp() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Warp");
	}

	public static GameObject GetWarpField() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Warp Field");
	}

	public static float GetRefireRate() {
		if (warpRefireRate == -1) {
			warpRefireRate = GetWarp ().GetComponent<Warp> ().refireRate;
		}
		return warpRefireRate;
	}

	public void SetOwner(GameObject newOwner) {
		owner = newOwner;
	}
}
