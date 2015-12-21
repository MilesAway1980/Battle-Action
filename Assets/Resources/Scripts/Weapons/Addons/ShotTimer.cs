using UnityEngine;
using System.Collections;

public class ShotTimer {

	float lastShot;

	public float getLastShot() {
		return lastShot;
	}

	public void updateLastShot() {
		lastShot = Time.fixedTime;
	}
}
