using UnityEngine;
using System.Collections;

public class Deactivator : Turret {

	// Use this for initialization
	void Start () {
		BulletShooter bs = GetComponent<BulletShooter>();
		bs.SetCurrentWeapon (13);
	}

	public new static GameObject GetTurret() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Deactivator");
	}
}
