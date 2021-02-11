using UnityEngine;
using Mirror;
using System.Collections;
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

	[SyncVar] Guid ownerGuid;
	
	void Start()
	{

		if (!isServer)
        {
			return;
        }

		name = $"Decoy: {ownerGuid}";

		Ship ship = Player.GetPlayerShip(ownerGuid);
		Owner owner = ship.GetComponent<Owner>();

		owner.AddDecoy();

		Owner decoyOwner = GetComponent<Owner>();
		decoyOwner.SetOwnerGuid(ownerGuid);
		decoyOwner.SetDecoy(true);

		damageable = GetComponent<Damageable>();
	}

	void OnDestroy()
	{
		print("on destroy " + ownerGuid);
		if (isServer)
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
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (damageable.GetArmor() <= 0)
        {
			Destroy(gameObject);
        }
	}

	public void Init(Guid ownerGuid)
	{
		this.ownerGuid = ownerGuid;
	}

	public static GameObject GetDecoy()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Decoy");
	}

	public static float GetRefireRate()
	{
		if (decoyRefireRate == -1)
		{
			decoyRefireRate = GetDecoy().GetComponent<Decoy>().refireRate;
		}

		return decoyRefireRate;
	}
}
