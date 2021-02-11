using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;

public class ArenaInfo : NetworkBehaviour
{

	int arenaSize = 400;
	int numBeacons = 50;
	int maxPowerups = 100;
	int startingPowerups = 10;
	float powerupRespawnRate = 2.0f;
	float beaconRange = 10;
	float shipRadarRange = 25;
	int minBulletTravelDist = 1000;
	int maxPlayers = 4;

	int numShips = 26;
	//int[] maxWeaponAmmo;

	List<WeaponData> weaponData;

	static ArenaInfo self;

	static GameObject[] shipList;
	static int numControllers = 1;

	void Awake()
	{
		if (self == null)
		{
			self = this;
		}

		if (this != self)
		{
			print("Destroy the duplicate!!");
			Destroy(this);  //There can only be one!
			return;
		}

		print(self);


		arenaSize = ScenePersistence.GetArenaSize();
		numBeacons = ScenePersistence.GetNumberOfBeacons();
		maxPowerups = ScenePersistence.GetMaxPowerups();
		startingPowerups = ScenePersistence.GetStartingPowerups();
		powerupRespawnRate = ScenePersistence.GetPowerupSpawnRate();
		beaconRange = ScenePersistence.GetBeaconRadarRange();
		shipRadarRange = ScenePersistence.GetShipRadarRange();
		//minBulletTravelDist
		maxPlayers = ScenePersistence.GetMaxPlayers();
		weaponData = ScenePersistence.GetWeaponData();

		if (isLocalPlayer)
		{
			print(ScenePersistence.GetArenaSize());
			print(ScenePersistence.GetNumberOfBeacons());
			print(ScenePersistence.GetMaxPlayers());
			print(ScenePersistence.GetMaxPowerups());
			print(ScenePersistence.GetStartingPowerups());
			print(ScenePersistence.GetPowerupSpawnRate());
			print(ScenePersistence.GetBeaconRadarRange());
			print(ScenePersistence.GetShipRadarRange());
			print(ScenePersistence.GetWeaponData());
		}

	}

	public static int GetArenaSize()
	{
		return self.arenaSize;
	}

	public static int GetMaxPlayers()
	{
		return self.maxPlayers;
	}

	public static int GetNumBeacons()
	{
		return self.numBeacons;
	}

	public static int GetMaxPowerups()
	{
		return self.maxPowerups;
	}

	public static int GetStartingPowerups()
	{
		return self.startingPowerups;
	}

	public static float GetPowerupRespawnRate()
	{
		return self.powerupRespawnRate;
	}

	public static float GetBeaconRange()
	{
		return self.beaconRange;
	}

	public static float GetShipRadarRange()
	{
		return self.shipRadarRange;
	}

	public static int GetNumControllers()
	{
		return numControllers;
	}

	public static int GetMinBulletTravelDist()
	{
		return self.minBulletTravelDist;
	}

	public static Vector2 GetRandomArenaLocation()
	{
		int size = self.arenaSize / 2;
		return new Vector2(
			Random.Range(-size, size),
			Random.Range(-size, size)
		);
	}

	public static WeaponData GetWeaponData(int whichWeapon)
	{
		return GetWeaponData(PowerupManager.GetPowerupName(whichWeapon));
	}

	public static WeaponData GetWeaponData(string weaponName)
	{
		return self.weaponData.Where(w => w.name.Trim().ToLower() == weaponName.Trim().ToLower()).FirstOrDefault();
	}

	public static List<WeaponData> GetWeaponData()
	{
		return self.weaponData;
	}
}
