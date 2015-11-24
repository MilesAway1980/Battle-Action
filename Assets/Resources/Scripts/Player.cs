using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour {

	//public GameObject test;

	static int playerCount = 0;

	GameObject ship;
	int playerNum;
	Camera followCam;

	GameObject newThing;
	NetworkHash128 regNum;

	void Awake() {
		print ("local = " + isLocalPlayer);
		//if (isLocalPlayer) {

			GameObject test = GameObject.CreatePrimitive (PrimitiveType.Cube);
			test.name = "Cube " + playerNum;
			
			test.AddComponent<NetworkIdentity> ();
			test.AddComponent<NetworkTransform> ();
			
			test.transform.position = new Vector2 (Random.Range (-4, 4), Random.Range (-4, 4));
			test.transform.localScale = new Vector3 (Random.Range (1, 3), Random.Range (1, 3), Random.Range (1, 3));

			newThing = (GameObject)Instantiate(test);
			Destroy (test);

			regNum = NetworkHash128.Parse (StringGenerator.randomString(32));
			print ("RegNum == " + regNum);
			ClientScene.RegisterSpawnHandler (regNum, onSpawn, onDespawn);

		//}

		InitState ();
	}

	GameObject onSpawn(Vector3 position, NetworkHash128 hashId) {



		return GameObject.CreatePrimitive (PrimitiveType.Capsule);
	}

	void onDespawn(GameObject obj) {
		Destroy (obj);
	}

	[Server]
	void InitState() {

		playerNum = ++playerCount;
		
		this.name = "Player " + playerNum;

		//GameObject newThing = (GameObject)Instantiate (test);




		/*Dictionary<NetworkHash128, GameObject> d = ClientScene.prefabs;
		foreach(KeyValuePair<NetworkHash128, GameObject> entry in d) {
			print (entry.Value + "\n" + entry.Key);
		}*/
		print ("Regnum: " + regNum);
		NetworkServer.Spawn (newThing, regNum);

		//Destroy (test);

		return;
		
		GameObject[] shipList = ArenaInfo.getShipList ();
		int whichShip = Random.Range (0, shipList.Length - 1);
		ship = (GameObject)Instantiate (shipList [whichShip]);
		ship.GetComponent<NetworkIdentity> ().GetInstanceID ();

		//NetworkIdentity NI = ship.AddComponent<NetworkIdentity>();
		//NI.localPlayerAuthority = true;
		//NetworkServer.Spawn (ship);

		//print (whichShip);
		//print (shipList[whichShip]);
		//print (ship);
	
		ship.name = "Player " + playerNum + " Ship";
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
		return playerNum;
	}
}
