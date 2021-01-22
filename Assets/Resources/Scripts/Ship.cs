using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum ShipController {
	none,
	player,
	decoy,
	computer
}

public class Ship : NetworkBehaviour {

	static public ObjectList shipList;					//A static list of all the ships in the game.

	[Header("The fastest the ship can travel")]
	public float maxSpeed;
	[Header("The maxiumum thrust output")]
	public float maxThrust;
	[Header("How much thrust the ship can increase in one frame")]
	public float acceleration;
	[Header("How quickly the ship can slow down in one frame")]
	public float deceleration;
	[Header("The maximum speed that the ship can turn")]
	public float maxTurnRate;
	[Header("How quickly the ship's turn rate changes from user input")]
	public float turnRateAcceleration;
	[Header("How quickly the ship's turn rate decreases when no user input")]
	public float turnRateDeceleration;
	[Header("How many weapons the ship can hold (not implemented)")]
	public int slots;
	[Header("The object that the ship is replaced with when destroyed")]
	public GameObject explosion;
	[Header("The gameobject that represents the ship.")]
	public GameObject shipModel;

	int turnDir;								//The direction the ship is turning (-1 - 0 - 1)

	[SyncVar] private float currentSpeed;		//The ship's current speed
	[SyncVar] private float thrust;				//How much thrust the ship currently has

	bool accelerating;							//The player is accelerating the ship
	bool decelerating;							//The player is decelerating the ship

	Vector2 currentPos;							//The ship's current position
	Vector2 oldPos;								//The ship's previous position

	float opponentDistance;						//The distance to the closest opponent
	Rigidbody2D rb;								//The ship's rigid body component

	[SyncVar] bool stopped;						//Whether or not the ship can move.
	[SyncVar] public ShipController shipController;     //who owns and operates the ship

	float turnTilt = 0;
	float turnRate;	

	void Awake() {

		if (shipList == null) {
			shipList = new ObjectList();
		}
		shipList.AddObject (gameObject);

		opponentDistance = -1;
	}

	void OnDestroy() {
		shipList.RemoveObject (gameObject);
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
		CheckDamage ();
	}

	void FixedUpdate() {
		if (stopped) {
			thrust = 0;
			return;
		}

		Accelerate ();
		Turn ();

		if (currentPos != oldPos) {
			oldPos = currentPos;
		}

		currentPos = transform.position;
	}

	void CheckDamage() {
		if (!isServer) {
			return;
		}

		Damageable dm = GetComponent<Damageable> ();

		if (dm) {
			if (dm.GetArmor() <= 0) {
				Explode();
				GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
				for (int i = 0; i < players.Length; i++) {
					Player player = players[i].GetComponent<Player>();

					HitInfo thisHitInfo = GetComponent<HitInfo> ();
					print (shipController);
					if ((player.GetPlayerNum() == thisHitInfo.GetLastHitBy()) &&
						(shipController == ShipController.player)) 
					{
						player.AddKill ();
						break;
					}
				}

				Destroy (gameObject);
			}	
		}
	}

	void Explode() {
		GameObject boom = Instantiate (explosion, transform.position, Quaternion.identity);
		Exploder exp = boom.GetComponent<Exploder> ();
		exp.Init (0.0f, 10);

		NetworkServer.Spawn (boom);
	}

	public void Accelerate() {

		if (accelerating) {
			IncreaseThrust();
		}

		if (decelerating) {
			DecreaseThrust();
		}

		rb.AddRelativeForce (new Vector2 (0, thrust));
			
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = rb.velocity.normalized * maxSpeed;
		}

		currentSpeed = rb.velocity.magnitude;
	}

	public void Turn() {
		rb.angularVelocity = 0;
		
		if (turnDir == 0)
		{
			if (turnRate != 0)
			{
				if (turnRate > 0)
                {
					turnRate -= turnRateDeceleration;
					if (turnRate < 0)
                    {
						turnRate = 0;
                    }
                }
				else
                {
					turnRate += turnRateAcceleration;
					if (turnRate > 0)
                    {
						turnRate = 0;
                    }
                }
			}
		}
		else
		{
			turnRate += turnRateAcceleration * turnDir;

			if (Mathf.Abs(turnRate) > maxTurnRate)
            {
				turnRate = maxTurnRate * turnDir;
            }
		}
			
        

		print(turnRate);


		float spinChange = turnRate;
		/*float tiltChange = spinChange;
		float currentTilt = transform.rotation.eulerAngles.y;
		float maxTilt = 50;

		if (turnDir == 0)   //Not turning
		{
			if (currentTilt > 0)
			{
				if (currentTilt > turnRate)
				{
					tiltChange = -turnRate;
				}
				else
                {
					tiltChange = -currentTilt;
                }
				
			}
			else if (currentTilt > 0)
			{
				if (currentTilt < -turnRate)
				{
					tiltChange = turnRate;
				} else
                {
					tiltChange = currentTilt;
                }
			}
			else
            {
				tiltChange = 0;
            }
		}
		else if (Mathf.Abs(currentTilt) >= maxTilt)
		{
			tiltChange = 0;
		}

		print(currentTilt + "   " + spinChange + "   " + tiltChange);
		*/

		if (spinChange != 0)
		{
			transform.Rotate(new Vector3(0, 0, spinChange));
		}

		/*if (tiltChange != 0)
		{
			transform.Rotate(new Vector3(
				0, tiltChange, 0
			));
		}
		*/
	}

	public float GetOpponentDistance() {
		return opponentDistance;

	}

	public float GetAngle() {
		return transform.eulerAngles.z;
	}

	public float GetSpeed() {
		return currentSpeed;
	}

	public void Accel(bool isAccelerating) {
		accelerating = isAccelerating;
	}

	public void Decel(bool isDecelerating) {
		decelerating = isDecelerating;
	}

	public void SetTurnDir(int direction) {
		if (direction < 0) {
			turnDir = -1;
		} else if (direction > 0) {
			turnDir = 1;
		} else {
			turnDir = 0;
		}
	}

	public void DecreaseThrust() {
		thrust -= deceleration;
		if (thrust < 0) {
			thrust = 0;
		}
	}

	public void IncreaseThrust() {
		thrust += acceleration;
		if (thrust > maxThrust) {
			thrust = maxThrust;
		}
	}

	public void SetShield(bool setting) {
		Shield shield = GetComponent<Shield> ();
		if (shield != null) {
			shield.SetActive(setting);
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
			dm.Damage (damage);			
		}
	}

	public float GetArmor() {
		
		Damageable dm = GetComponent<Damageable> ();
		if (dm) {
			return dm.GetArmor ();
		} else {
			return 0;
		}
	}

	public float GetThrust() {
		return thrust;
	}

	public float GetCurrentSpeed() {
		return currentSpeed;
	}

	public void SetStop (bool setting) {
		stopped = setting;
	}

	public bool GetStopped() {
		return stopped;
	}
}
