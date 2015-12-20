using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Mine : NetworkBehaviour {

	public float detectionRadius;
	public float damage;

	public float ownerScale;
	public float nonOwnerScale;

	//public GameObject exploder;

	[SyncVar] Vector3 pos;
	[SyncVar] Vector3 size;
	[SyncVar] int ownerNum;

	Player owner;
	Ship ownerShip;

	// Use this for initialization
	void Start () {		
		transform.position = pos;
		transform.localScale = Vector3.zero;
		if (isServer) {
			ownerNum = owner.getPlayerNum ();
		}
	}

	void Update() {

		if (owner != null) {
			if (ownerShip == null) {			
				ownerShip = owner.getShip ();
			}
		}

		Player thisPlayer = Player.getLocalPlayer ();
		if (thisPlayer == null) {
			return;
		}

		if (ownerNum == thisPlayer.getPlayerNum()) {
			size = new Vector3 (ownerScale, ownerScale, ownerScale);
		} else {
			size = new Vector3 (nonOwnerScale, nonOwnerScale, nonOwnerScale);
		}

		transform.localScale = size;
	}

	public static GameObject getMine() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Mine");
	}

	public void init(Player newOwner, Vector3 newPos) {		
		pos = newPos;
		owner = newOwner;
	}

	public float getDetectionRadius() {
		return detectionRadius;
	}

	public GameObject getExplosion() {		
		return (GameObject)Resources.Load ("Prefabs/Exploder");
	}

	public Vector2 getPos() {
		return pos;
	}
}
