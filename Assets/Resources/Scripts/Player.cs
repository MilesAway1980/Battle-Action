using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour {

	static int playerCount = 0;



	[SyncVar] int playerNum;

	Camera followCam;
	GameObject ship;

	void Awake() {
		InitState ();
	}

	void Start() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
		if (isLocalPlayer) {

			if (players != null) {
				for (int i = 0; i < players.Length; i++) {
					if (players[i].GetComponent<Ship>().getOwner() == playerNum) {
						ship = players[i];
						break;
					}
				}			
			}

			if (ship != null) {
				Ship myShip = ship.GetComponent<Ship>();
				myShip.makePointers();
				ShipCamera sc = ship.AddComponent<ShipCamera>();
				//sc.gameObject.AddComponent<StarField>();
				sc.setHeight(10);
				sc.setTarget(myShip.gameObject);
			}
		}
	}

	void Update() {
		/*if (followCam == null) {
			followCam = (Camera)GameObject.FindGameObjectWithTag ("MainCamera");
		}*/
		if (isLocalPlayer) {

			ship.GetComponent<Ship>().updateForLocal();

			/*

			Camera.main.transform.position = new Vector3 (
				ship.transform.position.x,
				ship.transform.position.y,
				-10
			);
			Ship myShip = ship.GetComponent<Ship>();*/

		}
	}

	void OnGUI() {

		if (isLocalPlayer) {
			string overlay = "";
			Ship thisShip = ship.GetComponent<Ship>();
			overlay += "Armor: " + thisShip.getArmor();
			overlay += "\nThrust: " + (int)thisShip.thrust + " \\ " + thisShip.maxThrust;
			overlay += "\nSpeed: " + (int)thisShip.currentSpeed + " \\ " + thisShip.maxSpeed;
			GUI.backgroundColor = new Color(0, 0, 0, 0);
			GUI.Box (new Rect (300, 10, 300, 100), "" + overlay);
		}


	}

	[Server]
	GameObject[] getPlayers() {
		return GameObject.FindGameObjectsWithTag ("Player Ship");
	}

	[Server]
	void InitState() {

		playerNum = ++playerCount;

		GameObject[] shipList = ArenaInfo.getShipList ();
		int whichShip = Random.Range (0, shipList.Length - 1);
		ship = (GameObject)Instantiate (
				shipList [whichShip],
				new Vector2(
					Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize()),
		             Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize())
		        ),
				Quaternion.identity
			);
		ship.transform.parent = this.transform;
		Ship thisShip = ship.GetComponent<Ship> ();
		thisShip.setOwner (playerNum);
		ship.tag = "Player Ship";
		ship.name = "playership" + playerNum;

		print (playerNum);

		NetworkServer.Spawn (ship);



		return;
		




		//ship.GetComponent<NetworkIdentity> ().GetInstanceID ();

		//NetworkIdentity NI = ship.AddComponent<NetworkIdentity>();
		//NI.localPlayerAuthority = true;
		//NetworkServer.Spawn (ship);

		//print (whichShip);
		//print (shipList[whichShip]);
		//print (ship);
	
		/*ship.name = "Player " + playerNum + " Ship";
		ship.transform.parent = transform;
		ship.GetComponent<Ship> ().setOwner (playerNum);
		
		Vector2 position = new Vector2(
			Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize()),
			Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize())
			);
		
		ship.transform.position = position;
		
		if (playerNum <= ArenaInfo.getNumControllers()) {
			Controls ctr = ship.AddComponent<Controls>();
			//ctr.setJoystick(playerNum);
			ctr.setJoystick(1);
		}
		
		ShipCamera shipCam = ship.AddComponent<ShipCamera>();
		followCam = shipCam.getCamera ();
		
		followCam.backgroundColor = new Color(0, 0, 0, 1);
		followCam.name = "Player " + playerNum + " Camera";
		//NetworkIdentity NI = followCam.gameObject.AddComponent<NetworkIdentity> ();
		//NI.localPlayerAuthority = true;
		//cam.transform.SetParent(playerShip.transform);
		
		shipCam.follow(true);
		shipCam.setHeight(10);
		shipCam.setTarget(ship);
		shipCam.setCullLayer(1 << 0 | 1 << (8 + playerNum));

		if (playerNum == 2) {

				//Top Player
				followCam.rect = new Rect(new Vector2(0, 0.5f), new Vector2(1, 0.5f));
			} else {
				//Bottom Player
				followCam.rect = new Rect(new Vector2(0, 0), new Vector2(1, 0.5f));

		}
		
		StarField sf = shipCam.gameObject.AddComponent<StarField>();
		sf.setStarLayer(8 + playerNum);
	}

	void OnDestroy() {
		if (followCam != null) {
			Destroy (followCam.gameObject);
		}*/
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

	/*public int getPlayerNum() {
		return playerNum;
	}*/
}
