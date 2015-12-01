using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Ship : NetworkBehaviour {

	[SyncVar] 	public float maxSpeed;
	[SyncVar] 	public float maxThrust;
	[SyncVar] 	public float acceleration;
				public float maxArmor;
	[SyncVar]	public float turnRate;
				public int slots;
				private float armor;
	[SyncVar]	public float currentSpeed;
				public float angle;
	[SyncVar]	public float thrust;

	[SyncVar]	float moveDist;
	[SyncVar]	float targetAngle;

	[SyncVar] 	int ownerNum;
	[SyncVar] 	public int turnDir;


	Rigidbody2D rb;
	ShipCamera shipCam;

	Vector2 currentPos;
	Vector2 oldPos;

	BulletShooter shooter;
	int currentWeapon;

	Vector2 axisPress; //temp

	static ObjectList shipList;

	GameObject beaconPointer;
	GameObject shipPointer;

	void Awake() {

		if (shipList == null) {
			shipList = new ObjectList();
		}
		shipList.addObject (this.gameObject);

		//gameObject.AddComponent<NetworkIdentity> ();
	}

	// Use this for initialization
	void Start () {
		//type

		armor = maxArmor;
		transform.Rotate (new Vector3 (0, 0, Random.Range (0, 360)));
		angle = getAngle ();
		targetAngle = angle;

		rb = GetComponent<Rigidbody2D> ();
		shipCam = GetComponent<ShipCamera> ();
		gameObject.tag = "Player Ship";

		currentPos = transform.position;
		oldPos = currentPos;

		moveDist = Vector2.Distance (currentPos, oldPos);

		shooter = gameObject.AddComponent<BulletShooter> ();
		shooter.setOwner (this);

		currentWeapon = 1;

		axisPress = new Vector2 (0, 0);
	}

	void FixedUpdate() {

		if (currentPos != oldPos) {
			moveDist = Vector2.Distance(currentPos, oldPos);
			oldPos = currentPos;
		}

		accelerate ();
		turn ();



		//StarField sf = GetComponent<ShipCamera> ();

		currentPos = transform.position;
	}

	public void makePointers() {
		GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		
		beaconPointer = (GameObject)Instantiate (sphere);
		beaconPointer.name = "Beacon Pointer";
		beaconPointer.transform.parent = transform;
		beaconPointer.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		
		shipPointer = (GameObject)Instantiate (sphere);
		shipPointer.name = "Player Pointer";
		shipPointer.transform.parent = transform;
		shipPointer.transform.localScale = new Vector3 (0.3f, 0.3f, 0.3f);
		
		Destroy (sphere);
	}

	public void setOwner(int newOwner) {
		ownerNum = newOwner;
	}

	public int getOwner() {
		return ownerNum;
	}
	
	// Update is called once per frame
	void Update () {



		//string overlay;

		//overlay = "Owner: " + ownerNum + "  " + transform.position;
		//overlay += "\nArmor: " + (int)armor;
		//overlay += "\nSpeed: " + (int)(currentSpeed *100);
		//overlay += "\nThrust: " + (int)(thrust * 100);

 		//overlay += "\nAxis: " + axisPress.x + ", " + axisPress.y;
		//overlay += "\nController Angle: " + angle + " / " + targetAngle;

		//shipCam.setScreenText (overlay);
		//shipCam.setSpeed (moveDist);
		//shipCam.setAngle (getAngle ());
	}


	public void updateForLocal() {
		//Updates for local network player only 

		//thrust = 10;
		//checkControls ();


		//CmdAccelerate ();
		//CmdTurn ();

		


		ObjectInfo closestBeacon = Beacon.getNearestBeacon (this.gameObject);
		float beaconAngle = (int)Angle.getAngle (transform.position, closestBeacon.pos);
		
		beaconPointer.transform.position = new Vector3 (
			transform.position.x + Mathf.Sin (beaconAngle / Mathf.Rad2Deg) * 2.5f,
			transform.position.y + Mathf.Cos (beaconAngle / Mathf.Rad2Deg) * 2.5f
			);
		
		ObjectInfo closestShip = shipList.getClosest (this.gameObject);
		
		if ( 
		    (closestBeacon.distance < ArenaInfo.getBeaconRange()) 	||
		    (closestShip.distance < ArenaInfo.getShipRadarRange()) 
		    )	
		{
			
			
			float shipAngle = (int)Angle.getAngle (transform.position, closestShip.pos);
			shipPointer.GetComponent<Renderer> ().enabled = true;
			shipPointer.transform.position = new Vector3 (
				transform.position.x + Mathf.Sin (shipAngle / Mathf.Rad2Deg) * 3.5f,
				transform.position.y + Mathf.Cos (shipAngle / Mathf.Rad2Deg) * 3.5f
				);
			//overlay += "\nClosest: " + (int)closestShip.distance;
			//overlay += "\nAngle: " + (int)Angle.getAngle (transform.position, closestShip.pos);
		} else {
			shipPointer.GetComponent<Renderer>().enabled = false;
		}

	}

	public void accelerate() {

		rb.AddRelativeForce (new Vector2 (0, thrust));
			
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}

		currentSpeed = rb.velocity.magnitude;
	}

	public void turn() {
		rb.angularVelocity = 0;
		transform.Rotate (new Vector3 (0, 0, turnRate * turnDir));
		angle = getAngle ();
		turnDir = 0;
	}

	public float getAngle() {
		Vector3 rot = transform.eulerAngles;
		return rot.z;
	}

	public float getSpeed() {
		return currentSpeed;
	}

	[Server]
	public void setTurnDir(int direction) {
		if (direction < 0) {
			turnDir = -1;
		} else if (direction > 0) {
			turnDir = 1;
		} 
	}

	[Server]
	public void decAccel() {
		thrust -= acceleration;
		if (thrust < 0) {
			thrust = 0;
		}

	}

	[Server]
	public void incAccel() {
		thrust += acceleration;
		if (thrust > maxThrust) {
			thrust = maxThrust;
		}
	}

	public void damage(float amount) {
		if (amount > 0) {
			armor -= amount;
			if (armor < 0) {

			}
		}
	}

	void OnCollisionEnter2D(Collision2D col) {
		GameObject objectHit = col.gameObject;

		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();
			float damage = (currentSpeed * rb.mass) / 10;
			shipHit.damage(damage);
			this.damage (damage);
		}

	}

	public float getArmor() {
		return armor;
	}
}
