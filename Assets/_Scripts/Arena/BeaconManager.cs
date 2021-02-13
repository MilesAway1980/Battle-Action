using UnityEngine;
using Mirror;
using System.Collections;

public class BeaconManager : NetworkBehaviour
{

	public GameObject beaconPrefab;
	static GameObject allBeacons;

	static bool done;

	// Use this for initialization

	void Start()
	{

	}

	void Update()
	{
		//if (Player.GetPlayerCount() > 0)
		//{
		PopulateBeacons();
		//}
	}

	void PopulateBeacons()
	{
		if (!isServer)
		{
			return;
		}

		if (allBeacons == null)
		{
			allBeacons = new GameObject();
			allBeacons.name = "Beacons";
		}

		int missing = ArenaInfo.GetNumBeacons() - Beacon.beaconList.Count;
		int turrets = Turret.turretList.Count;

		missing -= turrets;

		if (missing > 0)
		{
			for (int i = 0; i < missing; i++)
			{
				GameObject newBeacon = Instantiate(
					beaconPrefab,
					ArenaInfo.GetRandomArenaLocation(),
					Quaternion.identity
				);

				newBeacon.transform.parent = allBeacons.transform;

				NetworkServer.Spawn(newBeacon);
			}
		}
	}
}
