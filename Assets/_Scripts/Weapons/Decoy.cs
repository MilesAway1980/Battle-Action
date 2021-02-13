using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using System;


public class Decoy : NetworkBehaviour
{

	public float refireRate;
	static float decoyRefireRate = -1;

	public float speed;
	public float turnRate;

	Damageable damageable;

	[Tooltip("False = The drone sits still.  True = The drone flies to random locations")]
	public bool mobile;
	Vector2 destination;	
	
	Rigidbody2D rigidBody;

	[SyncVar] Guid ownerGuid;
	static List<Decoy> allDecoys = new List<Decoy>();
	
	void Start()
	{
		if (!isServer)
        {
			return;
        }

		allDecoys.Add(this);

		name = $"Decoy: {ownerGuid}";

		rigidBody = GetComponent<Rigidbody2D>();

		Ship ship = Player.GetPlayerShip(ownerGuid);
		Owner owner = ship.GetComponent<Owner>();
		//owner.AddDecoy();

		Owner decoyOwner = GetComponent<Owner>();
		decoyOwner.SetOwnerGuid(ownerGuid);
		decoyOwner.SetDecoy(true);

		damageable = GetComponent<Damageable>();

		if (mobile)
        {
			destination = ArenaInfo.GetRandomArenaLocation();
        }
	}

	void OnDestroy()
	{
		/*if (isServer)
		{
			print("Remove Decoy " + ownerGuid);
			Owner[] owners = FindObjectsOfType<Owner>();

			for (int i = 0; i < owners.Length; i++)
			{
				if (owners[i].GetOwnerGuid() == ownerGuid)
				{
					owners[i].RemoveDecoy();
					print("Remove Decoy from: " + owners[i].GetOwnerGuid());
				}
			}
		}*/
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!isServer)
        {
			return;
        }

		if (damageable.GetArmor() <= 0)
        {
			Destroy(gameObject);
        }
	
        if (mobile)
        {			
			if (Vector2.Distance(transform.position, destination) < rigidBody.velocity.magnitude)
            {
				destination = ArenaInfo.GetRandomArenaLocation();
            }


			float currentAngle = transform.rotation.eulerAngles.z;
			float angleToDestination = Angle.GetAngle(transform.position, destination, false);
			
			float angleDif = Mathf.Abs(currentAngle - angleToDestination);
						
			if (angleDif > turnRate)
            {
				int turnDir = Angle.GetDirection(transform.rotation.eulerAngles.z, angleToDestination);				
				transform.Rotate(new Vector3(0, 0, turnRate * turnDir));
            }
			else
            {
				transform.rotation = Quaternion.Euler(0, 0, angleToDestination);
            }

			float angleRad = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			rigidBody.velocity = new Vector2(
				-Mathf.Sin(angleRad) * speed,
				Mathf.Cos(angleRad) * speed
			);
        }
    }

    public void Init(Guid ownerGuid)
	{
		this.ownerGuid = ownerGuid;
	}

	public static GameObject GetDecoyPrefab()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Decoy");
	}

	public static float GetRefireRate()
	{
		if (decoyRefireRate == -1)
		{
			decoyRefireRate = GetDecoyPrefab().GetComponent<Decoy>().refireRate;
		}

		return decoyRefireRate;
	}

	[Server]
	public static Decoy[] GetPlayerDecoysByGuid(Guid playerGuid)
    {
		allDecoys.RemoveAll(d => d == null);
		return allDecoys.Where(d => d.ownerGuid == playerGuid).ToArray();
    }

}
