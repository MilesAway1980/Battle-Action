using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Decoy : NetworkBehaviour {

	public float refireRate;
	static float decoyRefireRate = -1;

	public float speed;
	public float turnRate;
	public float turnChange;

	public GameObject explosion;

	static Decoy thisDecoy;

	[SyncVar] public float armor;

	int ownerNum;
	Rigidbody2D rb;

	GameObject decoyShip;

	Player owner;
	Ship ownerShip;

	float turnCount;

	ObjectList ships;

	void Awake() {
		ships = Ship.shipList;
		ships.addObject (gameObject);
	}

	void OnDestroy() {
		ships.removeObject (gameObject);
	}

	// Use this for initialization
	void Start () {
		if (ownerShip == null) {
			return;
		}

		Vector3 pos = ownerShip.transform.position;
		float angle = ownerShip.getAngle ();

		transform.position = new Vector3 (
			pos.x - Mathf.Sin(angle / Mathf.Rad2Deg) * -2,
			pos.y + Mathf.Cos(angle / Mathf.Rad2Deg) * -2,
			0
		);

		this.name = "decoy" + owner.getPlayerNum();
		gameObject.tag = "Player Ship";

		ownerNum = owner.getPlayerNum ();
		rb = GetComponent<Rigidbody2D> ();

		turnCount = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position = new Vector3 (
			transform.position.x - Mathf.Sin(getAngle() / Mathf.Rad2Deg) * speed,
			transform.position.y + Mathf.Cos(getAngle() / Mathf.Rad2Deg) * speed,
			0
		);

		transform.Rotate (new Vector3 (0, 0, turnRate));
		turnCount += Mathf.Abs (turnRate);

		if (turnCount >= turnChange) {
			turnCount = 0;
			turnRate = -turnRate;
		}

		checkDamage ();
	}

	float getAngle() {
		return transform.eulerAngles.z;
	}

	public void init(Player newOwner, Ship newOwnerShip) {
		owner = newOwner;
		ownerShip = newOwnerShip;
	}

	[Server]
	public void damage(float amount) {
		if (amount > 0) {
			armor -= amount;
		}
	}

	void checkDamage() {
		if (!isServer) {
			return;
		}
		if (armor <= 0) {
			explode();
			Destroy (gameObject);
		}
	}

	void explode() {
		GameObject boom = (GameObject)Instantiate (explosion, transform.position, Quaternion.identity);
		Exploder exp = boom.GetComponent<Exploder> ();
		exp.init (0.0f, 10);

		NetworkServer.Spawn (boom);
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

	/*void OnCollisionEnter2D(Collision2D col) {
		if (!isServer) {
			return;
		}

		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship> ();
			Decoy decoyHit = objectHit.GetComponent<Decoy> ();
			float damage = (speed * rb.mass);
			if (shipHit) {
				shipHit.damage (damage);
				shipHit.setLastHitBy (ownerNum);
			}

			if (decoyHit) {
				decoyHit.damage (damage);
			}

			this.damage (damage);
		}

	}*/
}
