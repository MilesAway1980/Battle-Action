using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour {

	static int playerCount = 0;
	[SyncVar] int playerNum;

	bool buttonsReady = false;

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
				sc.setHeight(10);
				sc.setTarget(myShip.gameObject);
			}
		}
	}

	void Update() {
		if (isLocalPlayer) {
			ship.GetComponent<Ship>().updateForLocal();
			checkControls ();
		}
	}


	void checkControls() {
		
		Controls ctr = GetComponent<Controls> ();
		if (ctr == null) {
			ctr = gameObject.AddComponent<Controls> ();
			ctr.setJoystick (1);
		}
		
		JoystickButtons[] buttons = ctr.getButtons ();
		float [,] axis = ctr.getAxis();
		
		if (!buttonsReady) {
			if (buttons.Length != 0) {
				buttonsReady = true;
				for (int i = 0; i < buttons.Length; i++) {
					if (buttons[i] == null) {
						buttonsReady = false;
						return;
					}
				}
			}
		}
		
		int controlStyle = 0;

		float keyboardHorizontal = Input.GetAxis ("Horizontal");
		float keyboardVertical = Input.GetAxis ("Vertical");
		bool shootButton = Input.GetButton ("Shoot");
		
		if (controlStyle == 0) {
			
			if (	
			    buttons [1].getHeld() || 
			    shootButton
			    ) 
			{
				//Shoot
				//shooter.fireBullet(currentWeapon, transform.position, getAngle ());
			}
			
			if (
				buttons [4].getHeld () || 
				keyboardVertical < 0
				) 
			{
				//Slow down
				//myShip.decAccel();
				CmdSlowShip();
				
			}
			
			if (
				buttons [5].getHeld () || 
				keyboardVertical > 0
				) 
			{
				//Speed up
				//myShip.incAccel();
				CmdSpeedUpShip();
				
			}
			
			//turnDir = 0;
			
			if (
				buttons [6].getHeld () || 
				keyboardHorizontal < 0
				) 
			{
				//Turn Left
				CmdTurnShip (1);
				//myShip.turn (1);
				//turnDir = 1;
			}
			
			if (
				buttons [7].getHeld () || 
				keyboardHorizontal > 0
				) 
			{
				//Turn Right
				CmdTurnShip (-1);
				//turnDir = -1;
				//myShip.turn (-1);
			}
			
		} /*else if (controlStyle == 1) {
			
			if (axis [0, 0] != 0 || axis [0, 1] != 0) {
				
				//Get Throttle
				
				float curThrottle = 0;
				if (Mathf.Abs (axis [0, 0]) > Mathf.Abs (axis [0, 1])) {
					curThrottle = Mathf.Abs (axis [0, 0]);
				} else {
					curThrottle = Mathf.Abs (axis [0, 1]);
				}
				
				float curMaxThrust = curThrottle * maxThrust;
				if (thrust < curMaxThrust) {
					thrust += acceleration;
					if (thrust > curMaxThrust) {
						thrust = curMaxThrust;
					}
				} else if (thrust > curMaxThrust) {			
					thrust -= acceleration;
					if (thrust < 0) {
						thrust = 0;
					}
				}
				
				//Get Angle
				targetAngle = Angle.getAngle (
					new Vector2 (axis [0, 0], axis [0, 1]),
					Vector2.zero
					);
				
			} else {
				if (thrust > 0) {
					thrust -= acceleration;
					if (thrust < 0) {
						thrust = 0;
					}
				}
			}
			
			axisPress = new Vector2 (axis [0, 0], axis [0, 1]);
			if (getAngle () != targetAngle) {
				float angleDist = Mathf.Abs (getAngle () - targetAngle);
				if (angleDist < turnRate) {
					//angle = targetAngle;
				} else {
					int dirToTurn = Angle.getDirection (getAngle (), targetAngle, angleDist);
					//CmdTurn (dirToTurn);
				}
			}
			
		}*/
		//Vector2 axisPress = new Vector2(axis[0,0], axis[0, 1]);
		//float curThrottle = new Vector2 (axis[0,0], axis[0, 1]).normalized.magnitude;
		//print (axisPress);
		//print (curThrottle);
		//throttle = curThrottle * maxSpeed;
		
		
	}

	[Command]
	void CmdSpeedUpShip() {
		ship.GetComponent<Ship>().incAccel ();
	}

	[Command]
	void CmdSlowShip() {
		ship.GetComponent<Ship>().decAccel ();
	}

	[Command]
	void CmdTurnShip(int dir) {
		ship.GetComponent<Ship>().setTurnDir (dir);
	}


	void OnGUI() {

		if (isLocalPlayer) {
			string overlay = "";
			Ship thisShip = ship.GetComponent<Ship>();
			overlay += "Armor: " + thisShip.getArmor();
			overlay += "\nThrust: " + (int)thisShip.thrust + " \\ " + thisShip.maxThrust;
			overlay += "\nSpeed: " + (int)thisShip.currentSpeed + " \\ " + thisShip.maxSpeed;
			overlay += "\nTurn: " + thisShip.turnDir;
			GUI.backgroundColor = new Color(0, 0, 0, 0);
			GUI.Box (new Rect (300, 10, 300, 100), "" + overlay);
		}


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

		//NetworkIdentity playerNI = GetComponent<NetworkIdentity> ();

		//NetworkIdentity ni = ship.GetComponent<NetworkIdentity>();
		//ni.AssignClientAuthority(playerNI.connectionToClient);
		
		//print (playerNum);

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
