using UnityEngine;
using System;
using Mirror;

public class PlayerShipSpawner : NetworkBehaviour
{

	static GameObject playerContainer;
	static GameObject[] shipList;
	static GameObject testShip;

	static bool initialized = false;

	[Server]
	public static Ship SpawnShip(Guid playerGuid, int shipSelection)
	{		
		if (!initialized)
		{
			Init();
		}

		GameObject prefab;

		if (shipSelection != 1000)
        {
			prefab = shipList[shipSelection];
		}
		else
        {
			prefab = testShip;
        }

		GameObject shipObject = Instantiate(
			prefab,
			ArenaInfo.GetRandomArenaLocation(),
			Quaternion.identity
		);

		shipObject.transform.parent = playerContainer.transform;
		Ship newShip = shipObject.GetComponent<Ship>();

		newShip.RandomRotation();

		Owner owner = newShip.GetComponent<Owner>();
		if (owner)
		{
			owner.SetOwnerGuid(playerGuid);
		}

		BulletShooter shooter = shipObject.GetComponent<BulletShooter>();
		shooter.SetOwner(owner);
		shooter.SetCurrentWeapon(1);
		
		newShip.shipController = ShipController.player;

		shipObject.tag = "Player Ship";
		shipObject.name = $"Player Ship: {playerGuid}";

		NetworkServer.Spawn(shipObject);

		return newShip;
	}

	[Server]
	static void Init()
	{
		GameObject[] gameObjects = FindObjectsOfType<GameObject>();		
		for (int i = 0; i < gameObjects.Length; i++)
		{
			if (gameObjects[i].name == "Player Ships")
			{
				playerContainer = gameObjects[i];
			}
		}

		if (playerContainer == null)
		{
			playerContainer = new GameObject();
			playerContainer.name = "Player Ships";
		}

		LoadAvailablePlayerShipPrefabs();

		initialized = true;
	}

	static void LoadAvailablePlayerShipPrefabs()
	{
		if (shipList == null || shipList.Length == 0)
		{
			var allPrefabs = Resources.LoadAll("Prefabs/Ships", typeof(GameObject));
			shipList = new GameObject[allPrefabs.Length];

			for (int i = 0; i < allPrefabs.Length; i++)
			{
				GameObject shipObject = (GameObject)allPrefabs[i];
				if (shipObject.activeSelf)
				{
					if (shipObject.name != "Test_Ship")
					{
						int.TryParse(allPrefabs[i].name.Split('_')[1], out int shipNum);
						shipList[shipNum] = shipObject;
					}
                    else
                    {
						testShip = shipObject;
                    }
				}
			}
		}
	}
}
