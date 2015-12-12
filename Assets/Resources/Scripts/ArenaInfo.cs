using UnityEngine;
using System.Collections;

public static class ArenaInfo  {

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
	static int arenaSize = 10;//400;
	static int numBeacons = 1;//50;
=======
	static int arenaSize = 20;//0;
	static int numBeacons = 50;
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
=======
	static int arenaSize = 400;
=======
	static int arenaSize = 20;//0;
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
	static int numBeacons = 50;
>>>>>>> parent of 1dbc944... Added Intersect.cs, which adds the ability to check whether or not a line passes through a circle.  Altered the way bullets detect collisions by detecting if the line between the last position and the current position passes through the circle around the ship.  Extremely effective.  Also, completed Warp.
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
