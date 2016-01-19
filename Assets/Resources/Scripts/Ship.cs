using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Ship : NetworkBehaviour {

	static public ObjectList shipList;					//A static list of all the ships in the game.

	public float maxSpeed;						//The fastest the shop can travel
	public float maxThrust;						//The maximum thrust output
	public float acceleration;					//How much thrust the ship can increase/decrease in one frame

	public float turnRate;						//How quickly the ship can turn
	public int slots;							//How many weapons the ship can use (not implemented)

	private int turnDir;						//The direction the ship is turning (-1 - 0 - 1)

	public GameObject explosion;				//The explosion that plays when the ship is destroyed

	[SyncVar] private float currentSpeed;		//The ship's current speed
	[SyncVar] private float thrust;				//How much thrust the ship currently has

	private bool accelerating;					//The player is accelerating the ship
	private bool decelerating;					//The player is decelerating the ship

	private Vector2 currentPos;					//The ship's current position
	private Vector2 oldPos;						//The ship's previous position

	private float opponentDistance;				//The distance to the closest opponent
	private Rigidbody2D rb;						//The ship's rigid body component

	[SyncVar] bool stopped;						//Whether or not the ship can move.

	void Awake() {

		if (shipList == null) {
			shipList = new ObjectList();
		}
		shipList.addObject (this.gameObject);

		opponentDistance = -1;
	}

	void OnDestroy() {
		shipList.removeObject (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		//armor = maxArmor;
		transform.Rotate (new Vector3 (0, 0, Random.Range (0, 360)));

		rb = GetComponent<Rigidbody2D> ();

		gameObject.tag = "Player Ship";

		currentPos = transform.position;
		oldPos = currentPos;
		stopped = false;
	}

	void Update() {
		checkDamage ();
	}

	void FixedUpdate() {
		if (stopped) {
			thrust = 0;
			return;
		}

		accelerate ();
		turn ();

		if (currentPos != oldPos) {
			oldPos = currentPos;
		}

		currentPos = transform.position;
	}

	void checkDamage() {
		if (!isServer) {
			return;
		}

		Damageable dm = GetComponent<Damageable> ();

		if (dm) {
			if (dm.getArmor() <= 0) {
				explode();
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
				for (int i = 0; i < players.Length; i++) {
					Player p = players[i].GetComponent<Player>();

					HitInfo hi = GetComponent<HitInfo> ();

					if (p.getPlayerNum() == hi.getLastHitBy()) {
						p.addKill();
						break;
					}
				}
				Destroy (gameObject);
			}	
		}
	}

	void explode() {
		GameObject boom = (GameObject)Instantiate (explosion, transform.position, Quaternion.identity);
		Exploder exp = boom.GetComponent<Exploder> ();
		exp.init (0.0f, 10);

		NetworkServer.Spawn (boom);
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

		Damageable dm = objectHit.GetComponent<Damageable> ();

		if (dm) {
			float damage = (currentSpeed * rb.mass);
			dm.damage (damage);			
		}
	}

	public float getArmor() {
		
		Damageable dm = GetComponent<Damageable> ();
		if (dm) {
			return dm.getArmor ();
		} else {
			return 0;
		}
	}

	public float getThrust() {
		return thrust;
	}

	public float getCurrentSpeed() {
		return currentSpeed;
	}

	public void setStop (bool setting) {
		stopped = setting;
	}

	public bool getStopped() {
		return stopped;
	}
}
