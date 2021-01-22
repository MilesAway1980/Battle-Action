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
		print ("on destroy " + ownerNum);
		if (isServer) {
			print ("Remove Decoy " + ownerNum);
			Owner[] owners = Object.FindObjectsOfType<Owner> ();
			for (int i = 0; i < owners.Length; i++) {
				if (owners [i].GetOwnerNum () == ownerNum) {
					owners [i].RemoveDecoy ();
					print ("Remove Decoy from: " + owners [i].GetOwnerNum ());
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		if (owner == null) {
			return;
		}

		GameObject shipObject = Instantiate (owner.GetComponent<Ship> ().gameObject);
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
		ownerNum = info.GetOwnerNum ();
		info.EmptyDecoy ();
		if (info) {
			this.name = "decoy" + ownerNum;
		} else {
			this.name = "decoy";
		}
		print ("Add Decoy");
		Owner[] owners = Object.FindObjectsOfType<Owner> ();
		for (int i = 0; i < owners.Length; i++) {
			if (owners [i].GetOwnerNum () == ownerNum) {
				owners [i].AddDecoy ();
				print ("Add Decoy to: " + owners [i].GetOwnerNum ());
			}
		}

		Owner decoyOwner = shipObject.GetComponent<Owner> ();
		decoyOwner.SetOwnerNum (info.GetOwnerNum ());

		ship.transform.parent = this.transform;

		int num = ship.transform.childCount;
		for (int i = 0; i < num; i++) {

			GameObject child = ship.transform.GetChild (i).gameObject;

			if (child.tag == "Pointer" || child.tag == "Shield") {
				Destroy (child);
			}
		}

		Damageable dm = ship.GetComponent<Damageable> ();
		dm.SetArmor(decoyArmor);

		ship.shipController = ShipController.decoy;

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

	public void Init(GameObject newOwner) {
		owner = newOwner;
	}

	public static GameObject GetDecoy() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Decoy");
	}

	public static float GetRefireRate() {
		if (decoyRefireRate == -1) {
			decoyRefireRate = GetDecoy ().GetComponent<Decoy> ().refireRate;
		}

		return decoyRefireRate;
	}
}
