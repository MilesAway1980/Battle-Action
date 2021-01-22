using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Mine : NetworkBehaviour {

	public float detectionRadius;
	public float damage;

	public float ownerScale;
	public float nonOwnerScale;

	[SyncVar] Vector3 pos;
	[SyncVar] Vector3 size;
	[SyncVar] int ownerNum;

	GameObject owner;

	// Use this for initialization
	void Start () {		
		transform.position = pos;
		transform.localScale = Vector3.zero;
		if (isServer) {
			Owner ownerInfo = owner.GetComponent<Owner> ();
			if (ownerInfo) {
				ownerNum = ownerInfo.GetOwnerNum ();
			} else {
				ownerNum = -1;
			}
		}
	}

	void Update() {

		Player thisPlayer = Player.GetLocalPlayer ();
		if (thisPlayer == null) {
			return;
		}

		if (ownerNum == thisPlayer.GetPlayerNum()) {
			size = new Vector3 (ownerScale, ownerScale, ownerScale);
		} else {
			size = new Vector3 (nonOwnerScale, nonOwnerScale, nonOwnerScale);
		}

		transform.localScale = size;
	}

	public static GameObject GetMine() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Mine");
	}

	public void init(GameObject newOwner, Vector3 newPos) {		
		pos = newPos;
		owner = newOwner;
	}

	public float GetDetectionRadius() {
		return detectionRadius;
	}

	public GameObject GetExplosion() {		
		return (GameObject)Resources.Load ("Prefabs/Exploder");
	}

	public Vector2 GetPos() {
		return pos;
	}
}
