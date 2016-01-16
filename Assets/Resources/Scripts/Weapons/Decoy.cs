using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Decoy : NetworkBehaviour {

	public float refireRate;
	static float decoyRefireRate = -1;

	public float speed;
	public float turnRate;
	public float turnChange;

	public float decoyArmor;

	[SyncVar] int ownerNum;


	Ship ship;
	GameObject owner;

	float turnCount;

	void Awake() {		
	}

	void OnDestroy() {
		Owner[] owners = Object.FindObjectsOfType<Owner> ();
		for (int i = 0; i < owners.Length; i++) {
			if (owners [i].getOwnerNum () == ownerNum) {
				owners [i].removeDecoy ();
			}
		}
	}

	// Use this for initialization
	void Start () {
		if (owner == null) {
			return;
		}

		GameObject shipObject = (GameObject)Instantiate (owner.GetComponent<Ship> ().gameObject);
		ship = shipObject.GetComponent<Ship> ();

		Ship ownerShip = owner.GetComponent<Ship> ();

		Vector3 pos = ownerShip.transform.position;
		float angle = ownerShip.transform.eulerAngles.z;

		ship.transform.position = new Vector3 (
			pos.x - Mathf.Sin(angle / Mathf.Rad2Deg) * -2,
			pos.y + Mathf.Cos(angle / Mathf.Rad2Deg) * -2,
			0
		);

		Owner info = owner.GetComponent<Owner> ();
		ownerNum = info.getOwnerNum ();
		info.emptyDecoy ();
		if (info) {
			this.name = "decoy" + ownerNum;
		} else {
			this.name = "decoy";
		}

		Owner[] owners = Object.FindObjectsOfType<Owner> ();
		for (int i = 0; i < owners.Length; i++) {
			if (owners [i].getOwnerNum () == ownerNum) {
				owners [i].addDecoy ();
			}
		}

		Owner decoyOwner = shipObject.GetComponent<Owner> ();
		decoyOwner.setOwnerNum (info.getOwnerNum ());

		ship.transform.parent = this.transform;

		int num = ship.transform.childCount;
		for (int i = 0; i < num; i++) {

			GameObject child = ship.transform.GetChild (i).gameObject;

			if (child.tag == "Pointer" || child.tag == "Shield") {
				Destroy (child);
			}
		}

		Damageable dm = ship.GetComponent<Damageable> ();
		dm.setArmor (decoyArmor);

		NetworkServer.Spawn (shipObject);

		turnCount = 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (ship == null) {
			return;
		}

		float angle = ship.transform.eulerAngles.z;
		ship.transform.position = new Vector3 (
			ship.transform.position.x - Mathf.Sin(angle / Mathf.Rad2Deg) * speed,
			ship.transform.position.y + Mathf.Cos(angle / Mathf.Rad2Deg) * speed,
			0
		);

		ship.transform.Rotate (new Vector3 (0, 0, turnRate));
		turnCount += Mathf.Abs (turnRate);

		if (turnCount >= turnChange) {
			turnCount = 0;
			turnRate = -turnRate;
		}
	}

	public void init(GameObject newOwner) {
		owner = newOwner;
	}

	public static GameObject getDecoy() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Decoy");
	}

	public static float getRefireRate() {
		if (decoyRefireRate == -1) {
			decoyRefireRate = getDecoy ().GetComponent<Decoy> ().refireRate;
		}

		return decoyRefireRate;
	}
}
