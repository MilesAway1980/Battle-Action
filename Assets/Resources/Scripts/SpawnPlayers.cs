using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlayers : MonoBehaviour {

	public int numPlayers;
	public int numControllers;



	public GameObject[] ships;


	List<Player> playerList;

	//ObjectList playerList;

	// Use this for initialization
	/*void Start () {

		if (numPlayers == 0 || ships.Length == 0) {
			print ("No players or ships");
			Application.Quit ();
		}

		playerList = new List<Player>();

		for (int i = 0; i < numPlayers; i++) {

			int whichShip = Random.Range(0, ships.Length);
			//whichShip = 9;
			Vector2 newPos = new Vector2(
					Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize()),
					Random.Range (-ArenaInfo.getArenaSize(), ArenaInfo.getArenaSize())
				);

			Player newPlayer = new Player();
			playerList.Add(newPlayer);

			newPlayer.assignShip(ships[whichShip]);
			newPlayer.setPosition(newPos);

			GameObject playerShip = newPlayer.getShip();
			NetworkIdentity NI = playerShip.AddComponent<NetworkIdentity>();
			NI.localPlayerAuthority = true;


			//ShipList.addShip(playerShip.GetComponent<Ship>());

			if (i < numControllers) {
				Controls ctr = playerShip.AddComponent<Controls>();
				ctr.setJoystick(i + 1);
			}

			playerShip.AddComponent<ShipCamera>();
			ShipCamera shipCam = playerShip.GetComponent<ShipCamera>();
			Camera cam = shipCam.getCamera ();
			cam.backgroundColor = new Color(0, 0, 0, 1);
			cam.name = "Player " + i + " Camera";
			//cam.transform.SetParent(playerShip.transform);

			shipCam.follow(true);
			shipCam.setHeight(10);
			shipCam.setTarget(playerShip);
			shipCam.setCullLayer(1 << 0| 1 << (8 + i));

			StarField sf = shipCam.gameObject.AddComponent<StarField>();
			sf.setStarLayer(8 + i);

			if (numPlayers == 2) {
				if (i == 0) {
					//Top Player
					cam.rect = new Rect(new Vector2(0, 0.5f), new Vector2(1, 0.5f));
				} else {
					//Bottom Player
					cam.rect = new Rect(new Vector2(0, 0), new Vector2(1, 0.5f));
				}
			}

			if (numPlayers >= 3) {
				if (i == 0) {
					//Top Left Player
					cam.rect = new Rect(new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f));
				} else if (i == 1) {
					//Top Right Player
					cam.rect = new Rect(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
				} else if (i == 2) {
					//Bottom Left Player
					cam.rect = new Rect(new Vector2(0, 0), new Vector2(0.5f, 0.5f));
				} else {
					//Bottom Right Player
					cam.rect = new Rect(new Vector2(0.5f, 0), new Vector2(0.5f, 0.5f));
				}
			}
		}

		//PlayerList.setPlayerList (playerList);
	}*/
	
	// Update is called once per frame
	void Update () {
	
	}
}
