using UnityEngine;
using System.Collections;

public class ArenaInfo : MonoBehaviour
{

	public int arenaSize = 400;
	public int numBeacons = 50;
	public int numPowerups = 100;
	public float powerupRespawnRate = 2.0f;
	public float beaconRange = 10;
	public float shipRadarRange = 25;
	public int minBulletTravelDist = 1000;

	static ArenaInfo self;

	static GameObject[] shipList;
	static int numControllers = 1;

    void Start()
    {
		self = this;
    }


    public static int GetArenaSize()
	{
		return self.arenaSize;
	}

	public static int GetNumBeacons()
	{
		return self.numBeacons;
	}

	public static int GetNumPowerups()
	{
		return self.numPowerups;
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
}
