using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class MineField : NetworkBehaviour {

	[Range (1, 100)]
	public int minesPerDrop;
	public float radius;
	public float refireRate;
	static float mineRefireRate = -1;

	GameObject[] mines;

	[SyncVar] Vector2 pos;
	GameObject owner;
	int ownerNum;

	// Use this for initialization
	void Start () {		
		mines = new GameObject[minesPerDrop];
		PlaceMines ();
	}

	void PlaceMines() {
		if (!isServer) {
			return;
		}

		for (int i = 0; i < minesPerDrop; i++) {
			float distance = Random.Range (0.0f, radius);
			float angle = Random.Range (0.0f, 360.0f);

			Vector2 loc = new Vector2 (
				pos.x + Mathf.Cos((angle / Mathf.Rad2Deg)) * distance,
				pos.y + Mathf.Sin((angle / Mathf.Rad2Deg)) * distance
			);

			mines [i] = Instantiate (Mine.GetMine());
			mines [i].GetComponent<Mine>().init(owner, loc);

			mines [i].transform.parent = transform;

			NetworkServer.Spawn (mines [i]);
		}
	}

	public void Update() {
		
		if (!isServer) {
			return;
		}

		List<GameObject> ships = Damageable.damageableList.GetObjectList();

		if (owner == null) {
			owner = Ship.shipList.GetObjectByOwner (ownerNum);
		}

		//See if the mine field has any mines left.
		bool minesLeft = false;
		for (int i = 0; i < mines.Length; i++) {
			if (mines [i] != null) {
				minesLeft = true;
				break;
			}
		}

		//Destroy the mine field if there are no mines left.
		if (minesLeft == false) {
			Destroy (gameObject);
		}

		for (int i = 0; i < ships.Count; i++) {

			Ship ship = ships [i].GetComponent<Ship>();
			if (ship == null) {
				continue;
			}

			Owner shipOwner = ship.GetComponent<Owner>();
			if (shipOwner) {
				if (ownerNum == shipOwner.GetOwnerNum()) {
					continue;
				}
			}

			//Ship is within the mine field, check for mine hits
			if (Vector2.Distance (ship.transform.position, pos) < radius) {
				for (int m = 0; m < mines.Length; m++) {
					if (mines [m] == null) {
						continue;
					}
					Mine mine = mines [m].GetComponent<Mine> ();
					float detectionRadius = mine.GetDetectionRadius ();

					if (Vector2.Distance (ship.transform.position, mine.GetPos()) < detectionRadius) {
						//BOOM!

						GameObject explosion = Instantiate (mine.GetExplosion (), mine.GetPos(), Quaternion.identity);
						Exploder exp = explosion.GetComponent<Exploder> ();

						exp.Init (0, 2);

						NetworkServer.Spawn (explosion);

						Damageable dm = ship.GetComponent<Damageable> ();
						if (dm) {
							dm.Damage (mine.damage);
							HitInfo info = ship.GetComponent<HitInfo> ();
							if (info) {
								info.SetLastHitBy (owner);
							}
						}

						Destroy (mines[m].gameObject);
					}
				}
			}
		}
	}

	public void Init (GameObject newOwner, Vector2 newPos) {
		owner = newOwner;
		Owner thisOwner = newOwner.GetComponent<Owner> ();
		if (thisOwner) {
			ownerNum = thisOwner.GetOwnerNum ();
		} else {
			ownerNum = -1;
		}
		pos = newPos;
	}

	public static GameObject GetMineField() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/MineField");
	}

	public static float GetRefireRate() {
		if (mineRefireRate < 0) {
			mineRefireRate = MineField.GetMineField().GetComponent<MineField>().refireRate;
		}
		return mineRefireRate;
	}
}
