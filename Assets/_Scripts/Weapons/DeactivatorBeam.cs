using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class DeactivatorBeam : NetworkBehaviour {

	LightningBolt bolt;

	GameObject owner;
	GameObject target;

	public GameObject lightningBolt;
	public float deactivationTimer;

	public float range;

	[SyncVar] Vector3 targetPos;
	[SyncVar] Vector3 startPos;
	[SyncVar] bool visible;

	// Use this for initialization
	void Start () {
		bolt = lightningBolt.GetComponent<LightningBolt>();
		visible = false;
	}

	void OnDestroy() {
		if (target != null) {
			Ship ship = target.GetComponent<Ship>();
			if (ship) {
				ship.SetStop(false);
				BulletShooter bs = ship.GetComponent<BulletShooter>();
				if (bs) {
					bs.SetActive (true);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (visible) {
			bolt.enabled = true;
			bolt.transform.position = startPos;
			bolt.target = targetPos;
		}

		if (!isServer) {
			return;
		}

		if (owner == null) {
			Destroy (gameObject);
			return;
		}

		if (target == null) {
			target = Damageable.damageableList.GetClosest (owner);
		}

		if (target != null) {
			targetPos = target.transform.position;
			startPos = owner.transform.position;
			visible = true;

			if (Vector2.Distance (startPos, targetPos) > range) {
				Destroy (gameObject);
			} else {				
				Ship ship = target.GetComponent<Ship>();
				if (ship) {
					ship.SetStop (true);
					BulletShooter bs = ship.GetComponent<BulletShooter>();
					if (bs) {
						bs.SetActive (false);
					}
				}
			}
		} else {
			Destroy (gameObject);
		}
	}

	public void Init(GameObject newOwner) {
		owner = newOwner;
	}

	public static GameObject GetDeactivatorBeamPrefab() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Deactivator Beam");
	}
}
