using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

struct PlayerInfo {
	public GameObject ship;
	public int playerNum;
	public Camera followCam;
}

public class Player : NetworkBehaviour {

	static int playerCount = 0;

	//GameObject ship;
	//int playerNum;

	[SyncVar] PlayerInfo playerInfo;



	void Awake() {
		InitState ();
	}

	[Server]
	void InitState() {
		playerInfo.playerNum = ++playerCount;
		
		this.name = "Player " + playerInfo.playerNum;
		
		GameObject[] shipList = ArenaInfo.getShipList ();
		int whichShip = Random.Range (0, shipList.Length - 1);
		playerInfo.ship = (GameObject)Instantiate (shipList [whichShip]);

		print (whichShip);
		print (shipList[whichShip]);
		print (playerInfo.ship);

		playerInfo.ship.name = "Player " + playerInfo.playerNum + " Ship";
		playerInfo.ship.transform.parent = transform;
		playerInfo.ship.GetComponent<Ship> ().setOwner (playerInfo.playerNum);
		
		Vector2 position = new Vector2(
			Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize()),
			Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize())
			);
		
		playerInfo.ship.transform.position = position;
		
		if (playerInfo.playerNum <= ArenaInfo.getNumControllers()) {
			Controls ctr = playerInfo.ship.AddComponent<Controls>();
			//ctr.setJoystick(playerNum);
			ctr.setJoystick(1);
		}
		
		ShipCamera shipCam = playerInfo.ship.AddComponent<ShipCamera>();
		playerInfo.followCam = shipCam.getCamera ();
		
		playerInfo.followCam.backgroundColor = new Color(0, 0, 0, 1);
		playerInfo.followCam.name = "Player " + playerInfo.playerNum + " Camera";
		NetworkIdentity NI = playerInfo.followCam.gameObject.AddComponent<NetworkIdentity> ();
		NI.localPlayerAuthority = true;
		//cam.transform.SetParent(playerShip.transform);
		
		shipCam.follow(true);
		shipCam.setHeight(10);
		shipCam.setTarget(playerInfo.ship);
		shipCam.setCullLayer(1 << 0 | 1 << (8 + playerInfo.playerNum));

		if (playerInfo.playerNum == 2) {

				//Top Player
				playerInfo.followCam.rect = new Rect(new Vector2(0, 0.5f), new Vector2(1, 0.5f));
			} else {
				//Bottom Player
				playerInfo.followCam.rect = new Rect(new Vector2(0, 0), new Vector2(1, 0.5f));

		}
		
		StarField sf = shipCam.gameObject.AddComponent<StarField>();
		sf.setStarLayer(8 + playerInfo.playerNum);
	}

	void OnDestroy() {
		if (playerInfo.followCam != null) {
			Destroy (playerInfo.followCam.gameObject);
		}
	}

	/*public void assignShip(GameObject newShip) {
		if (ship != null) {
			GameObject.Destroy(ship);
		}

		ship = (GameObject)GameObject.Instantiate (newShip, position, Quaternion.identity);
		ship.name = "Player " + playerNum;
	}*/

	/*public GameObject getShip() {
		return ship;
	}*/

	/*public void setPosition(Vector2 newPos) {
		position = newPos;
		ship.transform.position = newPos;
	}

	public Vector2 getPosition() {
		return position;
	}*/

	public int getPlayerNum() {
		return playerInfo.playerNum;
	}
}
