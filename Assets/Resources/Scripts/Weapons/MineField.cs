using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MineField : NetworkBehaviour {

	[Range (1, 100)]
	public int minesPerDrop;
	public float radius;
	public float refireRate;
	static float mineRefireRate = -1;

	GameObject[] mines;

	[SyncVar] Vector2 pos;
	static float lastShot;

	Player owner;
	Ship ownerShip;

	// Use this for initialization
	void Start () {
		
		mines = new GameObject[minesPerDrop];

		placeMines ();
	}

	[Server]
	void placeMines() {
		
		for (int i = 0; i < minesPerDrop; i++) {
			float distance = Random.Range (0.0f, radius);
			float angle = Random.Range (0.0f, 360.0f);

			Vector2 loc = new Vector2 (
				pos.x + Mathf.Cos((angle / Mathf.Rad2Deg)) * distance,
				pos.y + Mathf.Sin((angle / Mathf.Rad2Deg)) * distance
			);

			mines [i] = (GameObject)Instantiate (Mine.getMine());
			mines [i].GetComponent<Mine>().init(owner, loc);

			mines [i].transform.parent = transform;

			NetworkServer.Spawn (mines [i]);
		}
	}

	public void Update() {
		
		if (!isServer) {
			return;
		}

		if (owner != null) {
			if (ownerShip == null) {			
				ownerShip = owner.getShip ();
			}
		}

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
		if (players == null) {
			return;
		}

		bool minesLeft = false;
		for (int i = 0; i < mines.Length; i++) {
			if (mines [i] != null) {
				minesLeft = true;
				break;
			}
		}
		//Destroy the mine field if there are no mines left.
		if (minesLeft == false) {
			Destroy (this.gameObject);
		}

		for (int i = 0; i < players.Length; i++) {
			Ship ship = players [i].GetComponent<Ship> ();
			if (ship == ownerShip) {
				continue;
			}

			//Ship is within the mine field, check for mine hits
			if (Vector2.Distance (ship.transform.position, pos) < radius) {
				for (int m = 0; m < mines.Length; m++) {
					if (mines [m] == null) {
						continue;
					}
					Mine mine = mines [m].GetComponent<Mine> ();
					float detectionRadius = mine.getDetectionRadius ();

					if (Vector2.Distance (ship.transform.position, mine.getPos()) < detectionRadius) {
						//BOOM!

						GameObject explosion = (GameObject)Instantiate (mine.getExplosion (), mine.getPos(), Quaternion.identity);
						Exploder exp = explosion.GetComponent<Exploder> ();

						exp.init (0, 2);

						NetworkServer.Spawn (explosion);

						ship.damage (mine.damage);
						Destroy (mines[m].gameObject);
						//mines [m] = null;
					}
				}
			}
		}
	}

	public static float getLastShot() {
		return lastShot;
	}

	public static void updateLastShot() {
		lastShot = Time.fixedTime;
	}

	public void init (Player newOwner, Vector2 newPos) {
		owner = newOwner;
		pos = newPos;
	}

	public static GameObject getMineField() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/MineField");
	}

	public static float getRefireRate() {
		if (mineRefireRate < 0) {
			mineRefireRate = MineField.getMineField().GetComponent<MineField>().refireRate;
		}
		return mineRefireRate;
	}
}
