using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

struct CurrentInfo {
	public bool shooting;
	public bool turningLeft;
	public bool turningRight;
	public bool shield;
	public bool speeding;
	public bool slowing;
}

public class Player : NetworkBehaviour {

	static Player thisPlayer;
	//static Ship thisPlayerShip;

	static int playerCount = 0;
	[SyncVar] int playerNum;
	[SyncVar] int kills;
	[SyncVar] float deadTimer;

	bool buttonsReady = false;

	GameObject ship;

	public float spawnDelay;

	//Use these as flags for holding down buttons so that commands aren't repeatedly sent.

	CurrentInfo current;

	Chat chatMessages;
	bool newMessage = false;

	bool showChatBox = false;
	string chatText;

	public SyncListString texts = new SyncListString();

	void Awake() {
		assignPlayerNum ();
	}

	void Start() {

		kills = 0;

		chatMessages = new Chat ();
		chatMessages.setMessageHistorySize (5);
		chatMessages.setMessageLifeSpan (10000);

		if (isLocalPlayer) {
			ShipCamera sc = gameObject.AddComponent<ShipCamera>();
			sc.setHeight(10);

			assignShip();
			setCameraToFollow();

			thisPlayer = this;
		}
		deadTimer = spawnDelay;
	}

	void Update() {
		//if (isServer) {
			if (isLocalPlayer) {
				if (ship != null) {
					Ship myShip = ship.GetComponent<Ship> ();
					myShip.updateForLocal ();
					checkControls ();					
				}					
			}

			if (ship == null) {		
				//Dead!

				deadTimer += Time.deltaTime;
				if (deadTimer >= spawnDelay) {					
					createShip ();
					assignShip ();
					setCameraToFollow ();
					if (ship != null) {
						deadTimer = 0;
					}
				}
			} 

		if (newMessage == true) {
			print ("Passing: " + chatMessages);
			if (chatMessages != null) {
				string[] messages = chatMessages.getMessages();
				if (messages != null) {
					print (messages.Length);
				}
			}
			CmdSendMessage(chatMessages);
			newMessage = false;
		}
		//}		
	}
	
	
	void checkControls() {
		
		Controls ctr = GetComponent<Controls> ();
		if (ctr == null) {
			ctr = gameObject.AddComponent<Controls> ();
			ctr.setJoystick (1);
		}
		
		JoystickButtons[] buttons = ctr.getButtons ();
		//float [,] axis = ctr.getAxis();
		
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

			for (int i = 1; i <= WeaponInfo.numWeapons(); i++) {
				string which = i.ToString();
				if (Input.GetKeyDown(which)) {
					CmdChangeWeapon(i);
				}
			}

			if (Input.GetKeyDown("escape")) {
				Application.Quit();
			}

			if (Input.GetKeyDown ("t")) {
				showChatBox = true;
			}

			if (Input.GetKeyDown ("enter")) {

				showChatBox = false;
			}


			if (	
			    buttons [1].getHeld() || 
			    shootButton
			    ) 
			{
				//CmdShootBullet();
				if (!current.shooting) {
					current.shooting = true;
					CmdSetShipToShoot(true);
				}
			} else {
				if (current.shooting) {
					current.shooting = false;
					CmdSetShipToShoot(false);
				}
			}


			if (
				buttons[2].getHeld () ||
				Input.GetKey ("z")
				)
			{
				if (!current.shield) {
					current.shield = true;
					CmdActivateShield(true);
				}
			} else {
				if (current.shield) {
					current.shield = false;
					CmdActivateShield(false);
				}
			}

			
			if (
				buttons [4].getHeld () || 
				keyboardVertical < 0
				) 
			{
				if (!current.slowing) {
					current.slowing = true;
					CmdSlowShip(true);
				}				
			} else {
				if (current.slowing) {
					current.slowing = false;
					CmdSlowShip(false);
				}
			}
			
			if (
				buttons [5].getHeld () || 
				keyboardVertical > 0
				) 
			{
				if (!current.speeding) {
					current.speeding = true;
					CmdSpeedUpShip(true);
				}				
			} else {
				if (current.speeding) {
					current.speeding = false;
					CmdSpeedUpShip(false);
				}
			}
			
			if (
				buttons [6].getHeld () || 
				keyboardHorizontal < 0
				) 
			{
				if (!current.turningLeft) {
					current.turningLeft = true;
					CmdTurnShip (1);
				}
			} else {
				if (current.turningLeft) {
					current.turningLeft = false;
					CmdTurnShip (0);
				}
			}

			
			if (
				buttons [7].getHeld () || 
				keyboardHorizontal > 0
				) 
			{
				if (!current.turningRight) {
					current.turningRight = true;
					CmdTurnShip (-1);
				}
			} else {
				if (current.turningRight) {
					current.turningRight = false;
					CmdTurnShip (0);
				}
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
	void CmdChangeWeapon(int which) {
		ship.GetComponent<Ship>().setCurrentWeapon(which);
	}

	[Command]
	void CmdDestroyShip() {
		ship.GetComponent<Ship> ().damage (10000);
	}

	[Command]
	void CmdSpeedUpShip(bool speedingUp) {
		ship.GetComponent<Ship>().Accel (speedingUp);
	}

	[Command]
	void CmdSlowShip(bool slowingDown) {
		ship.GetComponent<Ship>().Decel (slowingDown);
	}

	[Command]
	void CmdTurnShip(int dir) {
		ship.GetComponent<Ship>().setTurnDir (dir);
	}

	[Command]
	void CmdActivateShield(bool setting) {
		ship.GetComponent<Ship> ().setShield (setting);
	}

	[Command]
	void CmdSetShipToShoot(bool isShooting) {
		BulletShooter shooter = GetComponent<BulletShooter> ();
		//Ship myShip = ship.GetComponent<Ship> ();
		shooter.setOwner (this);
		shooter.setIsFiring (isShooting);
	}

	[Command]
	void CmdSendMessage(Chat sendMessages) {
		print ("Sending Message!");
		print (sendMessages);
		if (sendMessages != null) {
			string[] all = sendMessages.getMessages ();
			if (all != null) {
				print (all.Length);
			} else {
				print ("messages are null");
			}
		}
		RpcReceiveMessage (sendMessages);
	}

	[ClientRpc]
	void RpcReceiveMessage(Chat newChatMessage) {
		print ("Receiving message!");
		if (newChatMessage != null) {
			string[] messages = newChatMessage.getMessages();
			if (messages != null) {
				print (messages.Length);
			}
		} else {
			print (newChatMessage);
		}
		chatMessages = newChatMessage;
	}

	void OnGUI() {

		if (isLocalPlayer) {
			string overlay = "";
			if (ship != null) {
				Ship thisShip = ship.GetComponent<Ship>();

				float timeUntilReady = 0;
				float lastShot = 0; 
				int currentWeapon = thisShip.getCurrentWeapon ();

				switch (currentWeapon) {
					case 1: 
						timeUntilReady = MachineGun.getRefireRate (); 
						lastShot = MachineGun.getLastShot ();
						break;
					case 2: 
						timeUntilReady = Rocket.getRefireRate (); 
						lastShot = Rocket.getLastShot ();
						break;
					case 3: 
						timeUntilReady = Missile.getRefireRate (); 
						lastShot = Missile.getLastShot ();
						break;
					case 4: 
						timeUntilReady = 0; 
						lastShot = 0;
						break;
					case 5: 
						timeUntilReady = Crush.getRefireRate (); 
						lastShot = Crush.getLastShot ();
						break;
					case 6: 
						timeUntilReady = Nuke.getRefireRate (); 
						lastShot = Nuke.getLastShot ();
						break;
					case 7: 
						timeUntilReady = Warp.getRefireRate (); 
						lastShot = Warp.getLastShot ();
						break;
					case 8: 
						timeUntilReady = Plasma.getRefireRate (); 
						lastShot = Plasma.getLastShot ();
						break;
					case 9: 
						timeUntilReady = MineField.getRefireRate (); 
						lastShot = MineField.getLastShot ();
						break;
					//case 1: timeUntilReady = MachineGun.getRefireRate (); break;
					//case 1: timeUntilReady = MachineGun.getRefireRate (); break;
					//case 1: timeUntilReady = MachineGun.getRefireRate (); break;
				}

				timeUntilReady = (int)((timeUntilReady - (Time.fixedTime - lastShot)) * 1000);
				if (timeUntilReady < 0) {
					timeUntilReady = 0;
				}

				overlay += "Armor: " + (int)(thisShip.getArmor() * 10);
				overlay += "\nThrust: " + (int)(thisShip.getThrust() * 100) + " \\ " + (thisShip.maxThrust * 100);
				overlay += "\nSpeed: " + (int)(thisShip.getCurrentSpeed() * 100) + " \\ " + (thisShip.maxSpeed * 100);
				overlay += "\nWeapon: " + WeaponInfo.getWeaponName(thisShip.getCurrentWeapon())  + " " + timeUntilReady;

				Shield shield = thisShip.GetComponent<Shield>();
				overlay += "\n" + (int)shield.getCharge() + " \\ " + (int)shield.getMaxCharge();

				float opponentDistance = thisShip.getOpponentDistance();
				if (opponentDistance >= 0) {
					overlay += "\nOpponent: " + (int)(opponentDistance * 1000);
				}
			}

			if (showChatBox) {
				if (chatText == null) {
					chatText = "";
				}
				bool userHitReturn = false;
				Event e = Event.current;
				if (e.keyCode == KeyCode.Return) {
					userHitReturn = true;
					showChatBox = false;

					//chatMessages.newMessage(chatText);
					//newMessage = true;

					texts.Add(chatText);
				} 

				if (userHitReturn == false) {
					GUI.SetNextControlName("ChatText");
					chatText = GUI.TextField(new Rect(50, 200, 200, 50), chatText);
					GUI.FocusControl("ChatText");
				}
			}

			string chats = "";

			/*string[] allChatMessages = chatMessages.getMessages();
			if (allChatMessages != null) {
				for (int i = 0; i < allChatMessages.Length; i++) {
					chats += "\n" + i + " " +  allChatMessages[i];
				}
			}
			chatMessages.updateTimers();*/

			for (int i = 0; i < texts.Count; i++) {
				chats += "\n" + i + " " +texts[i];
				texts.Dirty(i);
			}


			/*GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");

			if (players != null) {
				overlay += "Players: " + players.Length;
				for (int i = 0; i < players.Length; i++) {
					Ship theShip = players[i].GetComponent<Ship>();
					if (theShip != null) {
						overlay += "\ni :   " + theShip.getArmor();
					}
				}
			} else {
				overlay += "Players is NULL";
			}*/

			//string killCount = "";

			/*killCount += "\nShield  : " + current.shield;
			killCount += "\nShooting : " + current.shooting;
			killCount += "\nSlowing  : " + current.slowing;
			killCount += "\nSpeeding : " + current.speeding;
			killCount += "\nLeft     : " + current.turningLeft;
			killCount += "\nRight   : " + current.turningRight;*/

			string killCount = "Kills: " + kills;
			//killCount += "\nRespawn:" + deadTimer + " / " + spawnDelay;
			GUI.backgroundColor = new Color(0, 0, 0, 0);
			GUI.Box (new Rect (300, 10, 300, 150), "" + overlay);
			GUI.Box (new Rect (200, 10, 300, 150), "" + killCount);
			GUI.Box (new Rect (0, 100, 400, 300), "" + chats);
		}


	}

	void assignPlayerNum() {
		playerNum = ++playerCount;
	}

	void createShip() {
		if (!isServer) {
			return;
		}
		GameObject[] shipList = ArenaInfo.getShipList ();

		int whichShip = Random.Range (0, shipList.Length);

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

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");

		if (players != null) {
			for (int i = 0; i < players.Length; i++) {
				if (players[i].GetComponent<Ship>().getOwnerNum() == playerNum) {
					ship = players[i];
					break;
				}
			}			
		}
	}

	void setCameraToFollow() {
		if (!isLocalPlayer) {
			//return;
		}

		ShipCamera sc = gameObject.GetComponent<ShipCamera> ();
		if (ship != null) {
			Ship myShip = ship.GetComponent<Ship> ();
			if (sc != null) {
				sc.setTarget (myShip.gameObject);
			}
		}

	}

	public Ship getShip() {
		if (ship != null) {
			return ship.GetComponent<Ship> ();
		} else {
			return null;
		}
	}

	public static Player getLocalPlayer() {
		return thisPlayer;
	}

	public int getPlayerNum() {
		return playerNum;
	}

	public void addKill() {
		kills++;
	}
}
