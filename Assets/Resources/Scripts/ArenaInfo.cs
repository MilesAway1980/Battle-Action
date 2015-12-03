using UnityEngine;
using System.Collections;

public static class ArenaInfo  {

	static int arenaSize = 200;
	static int numBeacons = 50;
	static int numPowerups = 100;
	static float powerupRespawnRate = 2.0f;
	static float beaconRange = 10;
	static float shipRadarRange = 25;
	static GameObject[] shipList;
	static int numControllers = 1;
	static int minBulletTravelDist = 1000;

	public static int getArenaSize() {
		return arenaSize;
	}

	public static int getNumBeacons() {
		return numBeacons;
	}

	public static int getNumPowerups() {
		return numPowerups;
	}

	public static float getPowerupRespawnRate() {
		return powerupRespawnRate;
	}

	public static float getBeaconRange() {
		return beaconRange;
	}

	public static float getShipRadarRange() {
		return shipRadarRange;
	}

	public static int getNumControllers() {
		return numControllers;
	}

	public static int getMinBulletTravelDist() {
		return minBulletTravelDist;
	}

	public static GameObject[] getShipList() {

		if (shipList == null) {
			shipList = new GameObject[10];

			for (int i = 0; i < 10; i++) {
				shipList[i] = (GameObject)Resources.Load("Prefabs/Ships/Ship_" + (i + 1), typeof(GameObject));
			}
		}

		return shipList;
	}
}
