using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Ship : NetworkBehaviour {

	static ObjectList shipList;					//A static list of all the ships in the game.

	public float maxSpeed;						//The fastest the shop can travel
	public float maxThrust;						//The maximum thrust output
	public float acceleration;					//How much thrust the ship can increase/decrease in one frame
	public float maxArmor;						//The ship's maximum armor
	public float turnRate;						//How quickly the ship can turn
	public int slots;							//How many weapons the ship can use (not implemented)

	private int turnDir;						//The direction the ship is turning (-1 - 0 - 1)

	public GameObject explosion;				//The explosion that plays when the ship is destroyed

	[SyncVar] private float armor;				//How much armor the ship currently has.
	[SyncVar] private float currentSpeed;		//The ship's current speed
	[SyncVar] private float thrust;				//How much thrust the ship currently has
	[SyncVar] private int ownerNum;				//The number of the player that owns the ship.
	[SyncVar] private int currentWeapon;		//The ship's currently selected weapon

	private bool freshKill;						//Signals to the player that their ship just earned a kill
	private bool accelerating;					//The player is accelerating the ship
	private bool decelerating;					//The player is decelerating the ship

	private int lastHitBy;						//The number of the player the ship was last hit by

	private Vector2 currentPos;					//The ship's current position
	private Vector2 oldPos;						//The ship's previous position

	private GameObject beaconPointer;			//The object that points to the closest beacon
	private GameObject shipPointer;				//The object that points to the closest ship

	private float opponentDistance;				//The distance to the closest opponent

	private Rigidbody2D rb;						//The ship's rigid body component

	void Awake() {

		if (shipList == null) {
			shipList = new ObjectList();
		}
		shipList.addObject (this.gameObject);

		opponentDistance = -1;
	}

	// Use this for initialization
	void Start () {
		armor = maxArmor;
		transform.Rotate (new Vector3 (0, 0, Random.Range (0, 360)));

		rb = GetComponent<Rigidbody2D> ();

		gameObject.tag = "Player Ship";

		currentPos = transform.position;
		oldPos = currentPos;

		currentWeapon = 1;
	}

	void Update() {
		checkDamage ();
	}

	void FixedUpdate() {

		accelerate ();
		turn ();

		if (currentPos != oldPos) {
			oldPos = currentPos;
		}

		currentPos = transform.position;
	}

	public void makePointers() {	

		beaconPointer = (GameObject)Instantiate(Resources.Load ("Prefabs/BeaconPointer"));
		beaconPointer.name = "Beacon Pointer";
		beaconPointer.transform.parent = transform;
		
		shipPointer = (GameObject)Instantiate(Resources.Load ("Prefabs/PlayerPointer"));
		shipPointer.name = "Player Pointer";
		shipPointer.transform.parent = transform;

		SpriteRenderer sr;

		sr = beaconPointer.GetComponent<SpriteRenderer> ();
		sr.transform.localScale = new Vector2 (0.15f, 0.15f);

		sr = shipPointer.GetComponent<SpriteRenderer> ();
		sr.transform.localScale = new Vector2 (0.1f, 0.1f);
	}

	void checkDamage() {
		if (!isServer) {
			return;
		}
		if (armor <= 0) {
			explode();
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < players.Length; i++) {
				Player p = players[i].GetComponent<Player>();
				if (p.getPlayerNum() == lastHitBy) {
					p.addKill();
					break;
				}
			}
			Destroy (gameObject);
		}
	}

	void explode() {
		GameObject boom = (GameObject)Instantiate (explosion, transform.position, Quaternion.identity);
		Exploder exp = boom.GetComponent<Exploder> ();
		exp.init (0.0f, 10);

		NetworkServer.Spawn (boom);
	}

	public void setOwner(int newOwner) {
		ownerNum = newOwner;
	}

	public int getOwnerNum() {
		return ownerNum;
	}

	public int getCurrentWeapon() {
		return currentWeapon ;
	}

	public void setCurrentWeapon(int whichWeapon) {
		if (currentWeapon > 0 && currentWeapon <= 12) {
			currentWeapon = whichWeapon;
		}
	}
	
	public void updateForLocal() {
		//Updates for local network player only 

		ObjectInfo closestBeacon = Beacon.getNearestBeacon (this.gameObject);
		float beaconAngle = (int)Angle.getAngle (transform.position, closestBeacon.pos);

		if (beaconPointer == null || shipPointer == null) {
			makePointers();
		}

		beaconPointer.transform.position = new Vector3 (
			transform.position.x + Mathf.Sin (beaconAngle / Mathf.Rad2Deg) * 1.8f,
			transform.position.y + Mathf.Cos (beaconAngle / Mathf.Rad2Deg) * 1.8f,
			-5
		);

		beaconPointer.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -beaconAngle));
		
		ObjectInfo closestShip = shipList.getClosest (this.gameObject);

		if ( 
		    ((closestBeacon.distance < ArenaInfo.getBeaconRange()) 	||
		    (closestShip.distance < ArenaInfo.getShipRadarRange())) &&
		    (closestShip.distance >= 0)
		    )	
		{
			float shipAngle = (int)Angle.getAngle (transform.position, closestShip.pos);
			opponentDistance = closestShip.distance;
			shipPointer.GetComponent<SpriteRenderer> ().enabled = true;
			shipPointer.transform.position = new Vector3 (
				transform.position.x + Mathf.Sin (shipAngle / Mathf.Rad2Deg) * 2.5f,
				transform.position.y + Mathf.Cos (shipAngle / Mathf.Rad2Deg) * 2.5f,
				-5
			);

			shipPointer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -shipAngle));
		} else {
			shipPointer.GetComponent<SpriteRenderer>().enabled = false;
			opponentDistance = -1;
		}
	}

	public void accelerate() {

		if (accelerating) {
			incThrust();
		}

		if (decelerating) {
			decThrust();
		}

		rb.AddRelativeForce (new Vector2 (0, thrust));
			
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}

		currentSpeed = rb.velocity.magnitude;
	}

	public void turn() {
		rb.angularVelocity = 0;
		transform.Rotate (new Vector3 (0, 0, turnRate * turnDir));
	}

	public float getOpponentDistance() {
		return opponentDistance;

	}

	public float getAngle() {
		return transform.eulerAngles.z;
	}

	public float getSpeed() {
		return currentSpeed;
	}

	public void Accel(bool isAccelerating) {
		accelerating = isAccelerating;
	}

	public void Decel(bool isDecelerating) {
		decelerating = isDecelerating;
	}

	public void setTurnDir(int direction) {
		if (direction < 0) {
			turnDir = -1;
		} else if (direction > 0) {
			turnDir = 1;
		} else {
			turnDir = 0;
		}
	}

	public void decThrust() {
		thrust -= acceleration;
		if (thrust < 0) {
			thrust = 0;
		}
	}

	public void incThrust() {
		thrust += acceleration;
		if (thrust > maxThrust) {
			thrust = maxThrust;
		}
	}

	[Server]
	public void damage(float amount) {
		if (amount > 0) {
			Shield shield = GetComponent<Shield>();
			if (shield.getActive()) {
				armor -= amount / 10;
				shield.damageShield(amount); 
			} else {
				armor -= amount;
			}
		}
	}

	public void setShield(bool setting) {
		Shield shield = GetComponent<Shield> ();
		if (shield != null) {
			shield.setActive(setting);
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (!isServer) {
			return;
		}

		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();
			float damage = (currentSpeed * rb.mass);
			shipHit.damage(damage);
			shipHit.setLastHitBy(ownerNum);
			this.damage (damage);
		}

	}

	public void setLastHitBy(int who) {
		lastHitBy = who;
	}

	public float getArmor() {
		return armor;
	}

	public float getThrust() {
		return thrust;
	}

	public float getCurrentSpeed() {
		return currentSpeed;
	}
}
