using UnityEngine;
using Mirror;
using System.Collections;

public class ShotTimer {

	[SyncVar] float lastShot;

	public float GetLastShot()
	{ 
		return lastShot;
	}

	public void UpdateLastShot()
	{
		lastShot = Time.fixedTime;
	}

	public void ReadyShot()
    {
		lastShot = float.MinValue;
    }
}
