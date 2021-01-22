using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ShotTimer {

	[SyncVar] float lastShot;

	public float GetLastShot() { 
		return lastShot;
	}

	public void UpdateLastShot() {	
		lastShot = Time.fixedTime;
	}
}
