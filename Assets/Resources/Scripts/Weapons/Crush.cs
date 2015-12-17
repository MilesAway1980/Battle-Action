using UnityEngine;
using System.Collections;

public class Crush : Bullet {

	static ShootingInfo crushInfo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameObject getBullet() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Crush");
	}
	
	public static new float getRefireRate() {
		if (crushInfo == null) {
			createCrushInfo();
		}
		return crushInfo.refireRate;
	}
	
	public static new float getBulletsPerShot() {
		if (crushInfo == null) {
			createCrushInfo();
		}
		return crushInfo.bulletsPerShot;
	}
	
	static void createCrushInfo() {
		Crush temp = Crush.getBullet ().GetComponent<Crush>();
		crushInfo = new ShootingInfo();
		crushInfo.bulletsPerShot = temp.bulletsPerShot;
		crushInfo.refireRate = temp.refireRate;
	}
}
