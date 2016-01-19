using UnityEngine;
using System.Collections;

public class Deactivator : Turret {

	// Use this for initialization
	void Start () {
		BulletShooter bs = GetComponent<BulletShooter> ();
		bs.setCurrentWeapon (13);
	}

	public new static GameObject getTurret() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Deactivator");
	}
}
