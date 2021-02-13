using UnityEngine;
using Mirror;
using System;
using System.Collections;

public enum ShipController
{
	none,
	player,
	computer
}

public class Ship : NetworkBehaviour
{

	static public ObjectList shipList = new ObjectList();                  //A static list of all the ships in the game.

	[Tooltip("The fastest the ship can travel")]
	public float maxSpeed;
	[Tooltip("The maxiumum thrust output")]
	public float maxThrust;
	[Tooltip("How much thrust the ship can increase in one frame")]
	public float acceleration;
	[Tooltip("How quickly the ship can slow down in one frame")]
	public float deceleration;
	[Tooltip("The maximum speed that the ship can turn")]
	public float maxTurnRate;
	[Tooltip("How quickly the ship's turn rate increases from user input")]
	public float turnRateAcceleration;
	[Tooltip("How quickly the ship's turn rate decreases when no user input")]
	public float turnRateDeceleration;
	[Tooltip("How slowly the ship can turn when using microturn")]
	public float microTurnRate;
	[Tooltip("How much the maximum amount of ammo is multiplied by")]
	public float ammoBoost;
	[Tooltip("The object that the ship is replaced with when destroyed")]
	public GameObject explosion;
	[Tooltip("The gameobject that represents the ship.")]
	public GameObject shipModel;

	int turnDir;                                //The direction the ship is turning (-1 - 0 - 1)
	bool microTurn;								//Turn the ship very slowly for precision aiming

	[SyncVar] private float currentSpeed;       //The ship's current speed
	[SyncVar] private float thrust;             //How much thrust the ship currently has

	bool accelerating;                          //The player is accelerating the ship
	bool decelerating;                          //The player is decelerating the ship

	Vector2 currentPos;                         //The ship's current position
	Vector2 oldPos;                             //The ship's previous position

	float opponentDistance;                     //The distance to the closest opponent
	Rigidbody2D rigidBody;                      //The ship's rigid body component

	[SyncVar] bool stopped;                     //Whether or not the ship can move.
	[SyncVar] public ShipController shipController;     //who owns and operates the ship

	//float turnTilt = 0;
	float turnRate;

	bool ammoInitialized = false;

	void Awake()
	{

		if (shipList == null)
		{
			shipList = new ObjectList();
		}
		shipList.AddObject(gameObject);

		opponentDistance = -1;
	}

	void OnDestroy()
	{
		shipList.RemoveObject(gameObject);
	}

	public static Ship GetShipByGuid(Guid playerGuid)
	{
		GameObject[] ships = shipList.GetObjectList().ToArray();		

		for (int i = 0; i < ships.Length; i++)
		{
			//Owner owner = ships[i].GetComponent<Owner>();
			Ship ship = ships[i].GetComponent<Ship>();
			if (ship.GetOwnerGuid() == playerGuid)
			{
				return ships[i].GetComponent<Ship>();
			}
		}

		return null;
	}

	public static Ship[] GetAllShips()
    {
		GameObject[] shipObjects = shipList.GetObjectList().ToArray();
		Ship[] ships = new Ship[shipObjects.Length];

		for (int i = 0; i < shipObjects.Length; i++) {
			ships[i] = shipObjects[i].GetComponent<Ship>();
		}

		return ships;
    }

	// Use this for initialization
	void Start()
	{
		//armor = maxArmor;
		rigidBody = GetComponent<Rigidbody2D>();

		gameObject.tag = "Player Ship";

		currentPos = transform.position;
		oldPos = currentPos;
		stopped = false;
	}

	void Update()
	{
		if (isServer)
		{			
			if (!ammoInitialized)
			{
				if (NetworkServer.active)
				{
					BulletShooter shooter = GetComponent<BulletShooter>();
					shooter.InitializeAmmo(ammoBoost);
					ammoInitialized = true;
				}
			}
		}

		CheckDamage();
	}

	void FixedUpdate()
	{
		if (stopped)
		{
			thrust = 0;
			return;
		}

		Accelerate();
		Turn();

		if (currentPos != oldPos)
		{
			oldPos = currentPos;
		}

		currentPos = transform.position;
	}

	public void RandomRotation()
	{
		transform.Rotate(new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));
	}

	void CheckDamage()
	{
		if (!isServer)
		{
			return;
		}

		Damageable dm = GetComponent<Damageable>();

		if (dm)
		{
			if (dm.GetArmor() <= 0)
			{
				Explode();
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				for (int i = 0; i < players.Length; i++)
				{
					Player player = players[i].GetComponent<Player>();

					HitInfo thisHitInfo = GetComponent<HitInfo>();
					if ((player.GetPlayerGuid() == thisHitInfo.GetLastHitBy()) &&
						(shipController == ShipController.player))
					{
						player.AddKill();
						break;
					}
				}

				Destroy(gameObject);
			}
		}
	}

	[Server]
	void Explode()
	{
		
		GameObject boom = Instantiate(explosion, transform.position, Quaternion.identity);
		Exploder exp = boom.GetComponent<Exploder>();
		exp.Init(0.0f, 10);

		NetworkServer.Spawn(boom);
	}

	public void Accelerate()
	{

		if (accelerating)
		{
			IncreaseThrust();
		}

		if (decelerating)
		{
			DecreaseThrust();
		}

		rigidBody.AddRelativeForce(new Vector2(0, thrust));

		if (rigidBody.velocity.magnitude > maxSpeed)
		{
			rigidBody.velocity = rigidBody.velocity.normalized * maxSpeed;
		}

		currentSpeed = rigidBody.velocity.magnitude;
	}

	public void Turn()
	{
		rigidBody.angularVelocity = 0;

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
					turnRate += turnRateDeceleration;
					if (turnRate > 0)
					{
						turnRate = 0;
					}
				}
			}
		}
		else
		{
			if (!microTurn)
			{
				turnRate += turnRateAcceleration * turnDir;

				if (Mathf.Abs(turnRate) > maxTurnRate)
				{
					turnRate = maxTurnRate * turnDir;
				}
			}
			else
            {
				if (Math.Abs(turnRate) > microTurnRate)
				{
					if (turnRate > microTurnRate)
					{
						turnRate -= turnRateDeceleration;
						if (turnRate <= microTurnRate)
						{
							turnRate = microTurnRate;
						}
					}

					if (turnRate < -microTurnRate)
					{
						turnRate += turnRateDeceleration;
						if (turnRate >= -microTurnRate)
						{
							turnRate = -microTurnRate;
						}
					}
				}
				else
				{
					turnRate += microTurnRate * turnDir;

					if (Mathf.Abs(turnRate) > microTurnRate)
					{
						turnRate = microTurnRate * turnDir;
					}
				}
			}
		}

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

	public float GetOpponentDistance()
	{
		return opponentDistance;
	}

	public float GetAngle()
	{
		return transform.eulerAngles.z;
	}

	public float GetSpeed()
	{
		return currentSpeed;
	}

	public float GetMaxSpeed()
	{
		return maxSpeed;
	}

	public void Accel(bool isAccelerating)
	{
		accelerating = isAccelerating;
	}

	public void Decel(bool isDecelerating)
	{
		decelerating = isDecelerating;
	}

	public void SetTurnDir(int direction)
	{
		if (direction < 0)
		{
			turnDir = -1;
		}
		else if (direction > 0)
		{
			turnDir = 1;
		}
		else
		{
			turnDir = 0;
		}
	}

	public void SetMicroTurn(bool micro)
    {
		microTurn = micro;
    }

	public void DecreaseThrust()
	{
		thrust -= deceleration;
		if (thrust < 0)
		{
			thrust = 0;
		}
	}

	public void IncreaseThrust()
	{
		thrust += acceleration;
		if (thrust > maxThrust)
		{
			thrust = maxThrust;
		}
	}

	public void SetShield(bool setting)
	{
		Shield shield = GetComponent<Shield>();
		if (shield != null)
		{
			shield.SetActive(setting);
		}
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (!isServer)
		{
			return;
		}

		Damageable dm = collision.gameObject.GetComponent<Damageable>();

		if (dm)
		{
			Owner hitOwner = collision.gameObject.GetComponent<Owner>();
			Guid shipGuid = GetOwnerGuid();

			if (hitOwner.GetOwnerGuid() != shipGuid)
			{
				float damage = (currentSpeed * rigidBody.mass);
				dm.Damage(damage);				

				HitInfo hitInfo = collision.gameObject.GetComponent<HitInfo>();
				if (hitInfo)
                {
					hitInfo.SetLastHitBy(shipGuid);
                }
			}
		}
	}

	[Server]
	public bool HasDecoy()
    {
		return Decoy.GetPlayerDecoysByGuid(GetOwnerGuid()).Length > 0;
    }

	public Guid GetOwnerGuid()
    {
		Owner owner = GetComponent<Owner>();
		if (owner)
        {
			return owner.GetOwnerGuid();
        }

		return Guid.Empty;
    }

	public float GetThrust()
	{
		return thrust;
	}

	public float GetMaxThrust()
	{
		return maxThrust;
	}

	public void SetStop(bool setting)
	{
		stopped = setting;
	}

	public bool GetStopped()
	{
		return stopped;
	}

	public float GetAmmoBoost()
	{
		return ammoBoost;
	}
}
