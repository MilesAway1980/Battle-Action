using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShotTimer {

	[SyncVar] float lastShot;

	public float getLastShot() { 
		return lastShot;
	}

	public void updateLastShot() {	
		lastShot = Time.fixedTime;
	}
}
