using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Player : NetworkBehaviour {

	static int playerCount = 0;
	[SyncVar] int playerNum;

	[SyncVar] int kills;

	bool buttonsReady = false;

	GameObject ship;

	float deadTimer;

	void Awake() {
		assignPlayerNum ();
		createShip ();
	}

	void Start() {
		kills = 0;
		if (isLocalPlayer) {
			ShipCamera sc = gameObject.AddComponent<ShipCamera>();
			sc.setHeight(10);

			assignShip();
			setCameraToFollow();
		}
		deadTimer = -1;
	}

	void Update() {
		if (isLocalPlayer) {
			if (ship != null) {
				Ship myShip = ship.GetComponent<Ship>();
				myShip.updateForLocal();
				checkControls ();
			} 
		}

		if (ship == null) {
		
				//Dead!
				if (deadTimer == -1) {
					deadTimer = Time.fixedTime;
				}
				
				if ((Time.fixedTime - deadTimer) > 3) {
					deadTimer = -1;
					createShip ();
					assignShip ();
					setCameraToFollow();
				}

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

		int whichJoy = ctr.getJoystick ();
		for (int i = 0; i < buttons.Length; i++) {		
			buttons[i].setHeld (Input.GetButton("Joy" + whichJoy + "_Button" + (i + 1)));
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
				CmdShootBullet();
			}
			
			if (
				buttons [4].getHeld () || 
				keyboardVertical < 0
				) 
			{
				//Slow down
				CmdSlowShip();
				
			}
			
			if (
				buttons [5].getHeld () || 
				keyboardVertical > 0
				) 
			{
				//Speed up
				CmdSpeedUpShip();
				
			}
			
			if (
				buttons [6].getHeld () || 
				keyboardHorizontal < 0
				) 
			{
				//Turn Left
				CmdTurnShip (1);
			}
			
			if (
				buttons [7].getHeld () || 
				keyboardHorizontal > 0
				) 
			{
				//Turn Right
				CmdTurnShip (-1);
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

	[Command]
	void CmdShootBullet() {

		Ship myShip = ship.GetComponent<Ship>();
		//shooter.test ();
		BulletShooter shooter = GetComponent<BulletShooter> ();
		shooter.setOwner (myShip);

		shooter.fireBullet(
			myShip.getCurrentWeapon(), 
			myShip.transform.position, 
			myShip.getAngle ()
		);

	}

	void OnGUI() {

		if (isLocalPlayer) {
			string overlay = "";
			if (ship != null) {
				Ship thisShip = ship.GetComponent<Ship>();
				overlay += "Armor: " + (int)(thisShip.getArmor() * 10);
				overlay += "\nThrust: " + (int)thisShip.thrust + " \\ " + thisShip.maxThrust;
				overlay += "\nSpeed: " + (int)thisShip.currentSpeed + " \\ " + thisShip.maxSpeed;
				float opponentDistance = thisShip.getOpponentDistance();
				if (opponentDistance >= 0) {
					overlay += "\nOpponent: " + (int)(opponentDistance * 1000);
				}
			}

			string killCount = "Kills: " + kills;
			GUI.backgroundColor = new Color(0, 0, 0, 0);
			GUI.Box (new Rect (300, 10, 300, 100), "" + overlay);
			GUI.Box (new Rect (200, 10, 300, 100), "" + killCount);
		}


	}

	[Server]
	void assignPlayerNum() {
		playerNum = ++playerCount;
	}

	[Server]
	void createShip() {
		GameObject[] shipList = ArenaInfo.getShipList ();
		int whichShip = Random.Range (0, shipList.Length - 1);

		ship = (GameObject)Instantiate (
				shipList [whichShip],
				new Vector2 (
					Random.Range (-ArenaInfo.getArenaSize (), ArenaInfo.getArenaSize ()),
		             Random.Range (-ArenaInfo.getArenaSize (), ArenaInfo.getArenaSize ())
				),
				Quaternion.identity
		);

		ship.transform.parent = this.transform;
		Ship thisShip = ship.GetComponent<Ship> ();

		thisShip.setOwner (playerNum);
		ship.tag = "Player Ship";
		ship.name = "playership" + playerNum;

		NetworkServer.Spawn (ship);
	}

	void assignShip() {
		if (!isLocalPlayer) {
			return;
		}

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");

		if (players != null) {
			for (int i = 0; i < players.Length; i++) {
				if (players[i].GetComponent<Ship>().getOwner() == playerNum) {
					ship = players[i];
					break;
				}
			}			
		}
		if (ship != null) {
			Ship myShip = ship.GetComponent<Ship> ();
			myShip.makePointers ();
		}
	}

	void setCameraToFollow() {
		if (!isLocalPlayer) {
			return;
		}

		ShipCamera sc = gameObject.GetComponent<ShipCamera> ();
		Ship myShip = ship.GetComponent<Ship>();

		sc.setTarget (myShip.gameObject);

	}

	public int getPlayerNum() {
		return playerNum;
	}

	public void addKill() {
		kills++;
	}
}
