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

	static int playerCount = 0;
	[SyncVar] int playerNum;
	[SyncVar] int kills;
	[SyncVar] float deadTimer;

	bool buttonsReady = false;

	GameObject ship;

	public float spawnDelay;

	bool pointersActive;

	//Use these as flags for holding down buttons so that commands aren't repeatedly sent.

	CurrentInfo current;
	MouseControl mouseControl;

	Chat chatMessages;
	bool newMessage = false;

	bool showChatBox = false;
	string chatText;

	static GameObject[] shipList;

	public SyncListString texts = new SyncListString();

	void Awake() {
		AssignPlayerNum();
		GetAvailablePlayerShipList();
	}

	void Start() {

		kills = 0;

		chatMessages = new Chat ();
		chatMessages.SetMessageHistorySize (5);
		chatMessages.SetMessageLifeSpan (10000);

		if (isLocalPlayer) {
			ShipCamera sc = gameObject.AddComponent<ShipCamera>();
			sc.SetHeight(10);

			AssignShip();
			SetCameraToFollow();

			thisPlayer = this;
		}

		deadTimer = spawnDelay;
		mouseControl = gameObject.AddComponent<MouseControl> ();
		pointersActive = false;
	}

	void Update() {
		
		if (isLocalPlayer) {
			if (ship != null) {				
				CheckControls ();

				if (pointersActive == false) {
					Pointer[] pointers = ship.GetComponents<Pointer> ();
					for (int i = 0; i < pointers.Length; i++) {
						pointers [i].SetActive (true);
					}
					pointersActive = true;
				}
			}					
		}

		if (ship == null) {		
			//Dead!
			pointersActive = false;
			deadTimer += Time.deltaTime;
			if (deadTimer >= spawnDelay) {					
				CreateShip ();
				AssignShip ();
				SetCameraToFollow ();
				if (ship != null) {
					deadTimer = 0;
				}
			}
		} 

		if (newMessage == true) {
			print ("Passing: " + chatMessages);
			if (chatMessages != null) {
				string[] messages = chatMessages.GetMessages();
				if (messages != null) {
					print (messages.Length);
				}
			}
			CmdSendMessage(chatMessages);
			newMessage = false;
		}
	}
	
	
	void CheckControls() {

		if (mouseControl.button [0]) {
			//A little fancy math to get the angles to line up.
			float clickAngle = 180 - Angle.GetAngle (
				mouseControl.pos,
				new Vector2 (Screen.width / 2, Screen.height / 2)
			);
			if (clickAngle < 0) {
				clickAngle += 360;
			}
		}
		
		Controls ctr = GetComponent<Controls> ();
		if (ctr == null) {
			ctr = gameObject.AddComponent<Controls> ();
			ctr.SetJoystick (1);
		}
		
		JoystickButtons[] buttons = ctr.GetButtons ();
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



		int whichJoy = ctr.GetJoystick ();
		for (int i = 0; i < buttons.Length; i++) {		
			buttons[i].SetHeld (Input.GetButton("Joy" + whichJoy + "_Button" + (i + 1)));
		}

		int controlStyle = 0;

		float keyboardHorizontal = Input.GetAxis ("Horizontal");
		float keyboardVertical = Input.GetAxis ("Vertical");
		bool shootButton = Input.GetButton ("Shoot");

		if (controlStyle == 0) {

			for (int i = 1; i <= WeaponInfo.GetWeaponCount(); i++) {
				string which = "";
				if (i <= 9) {
					which = i.ToString ();
				} else {
					switch (i) {
						case 10: which = "u"; break;
						case 11: which = "i"; break;
						case 12: which = "o"; break;
						case 13: which = "p"; break;
					}
				}

				if (Input.GetKeyDown (which)) {
					CmdChangeWeapon (i);
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
			    buttons [1].GetHeld() || 
			    shootButton ||
				mouseControl.button[1]
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
				buttons[2].GetHeld () ||
				Input.GetKey ("z") ||
				mouseControl.button[3]
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
				buttons [4].GetHeld () || 
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
				buttons [5].GetHeld () || 
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
				buttons [6].GetHeld () || 
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
				buttons [7].GetHeld () || 
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
		BulletShooter shooter = ship.GetComponent<BulletShooter> ();
		shooter.SetCurrentWeapon (which);
	}

	[Command]
	void CmdDestroyShip() {
		//ship.GetComponent<Ship> ().damage (10000);
		Damageable d = ship.GetComponent<Damageable>();
		if (d) {
			d.Damage (100000);
		}
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
		ship.GetComponent<Ship>().SetTurnDir (dir);
	}

	[Command]
	void CmdActivateShield(bool setting) {
		ship.GetComponent<Ship>().SetShield (setting);
	}

	[Command]
	void CmdSetShipToShoot(bool isShooting) {
		BulletShooter shooter = ship.GetComponent<BulletShooter> ();
		shooter.SetIsFiring (isShooting);
	}

	[Command]
	void CmdSendMessage(Chat sendMessages) {
		print ("Sending Message!");
		print (sendMessages);
		if (sendMessages != null) {
			string[] all = sendMessages.GetMessages ();
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
			string[] messages = newChatMessage.GetMessages();
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
				BulletShooter shooter = ship.GetComponent<BulletShooter> ();

				Owner info = thisShip.GetComponent<Owner> ();

				float timeUntilReady = 0;
				float lastShot = 0; 
				int currentWeapon = shooter.GetCurrentWeapon ();

				if (shooter != null) {
					lastShot = shooter.GetLastShot (currentWeapon);
					//print (lastShot);
				} 

				switch (currentWeapon) {
					case 1: timeUntilReady = MachineGun.GetRefireRate (); break;
					case 2: timeUntilReady = Rocket.GetRefireRate (); break;
					case 3: timeUntilReady = Missile.GetRefireRate (); break;
					case 4: timeUntilReady = 0; break;
					case 5: timeUntilReady = Crush.GetRefireRate (); break;
					case 6: timeUntilReady = Nuke.GetRefireRate (); break;
					case 7: timeUntilReady = Warp.GetRefireRate (); break;
					case 8: timeUntilReady = Plasma.GetRefireRate (); break;
					case 9: timeUntilReady = MineField.GetRefireRate (); break;
					case 10: timeUntilReady = Decoy.GetRefireRate (); break;
					case 11: timeUntilReady = Turret.GetRefireRate (); break;
					case 12: timeUntilReady = Deactivator.GetRefireRate (); break;
				}

				timeUntilReady = (int)((timeUntilReady - (Time.fixedTime - lastShot)) * 1000);
				if (timeUntilReady < 0) {
					timeUntilReady = 0;
				}

				overlay += "Armor: " + (int)(thisShip.GetArmor() * 10);
				overlay += "\nThrust: " + (int)(thisShip.GetThrust() * 100) + " \\ " + (thisShip.maxThrust * 100);
				overlay += "\nSpeed: " + (int)(thisShip.GetCurrentSpeed() * 100) + " \\ " + (thisShip.maxSpeed * 100);
				overlay += "\nWeapon: " + WeaponInfo.GetWeaponName (currentWeapon) + " " + timeUntilReady + "  " + lastShot + " decoys: " + info.GetNumDecoy();

				Shield shield = thisShip.GetComponent<Shield>();
				overlay += "\n" + (int)shield.GetCharge() + " \\ " + (int)shield.GetMaxCharge();

				float opponentDistance = thisShip.GetOpponentDistance();
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

	void AssignPlayerNum() {
		playerNum = ++playerCount;
	}

	void CreateShip() {
		if (!isServer) {
			return;
		}

		int whichShip = Random.Range(0, shipList.Length);

		ship = Instantiate (
				shipList[whichShip],
				ArenaInfo.GetRandomArenaLocation(),
				Quaternion.identity
		);

		ship.transform.parent = this.transform;
		Ship thisShip = ship.GetComponent<Ship>();

		BulletShooter shooter = ship.GetComponent<BulletShooter> ();
		shooter.SetOwner (ship);
		shooter.SetCurrentWeapon (13);

		Owner owner = thisShip.GetComponent<Owner>();
		if (owner) {
			owner.SetOwnerNum (playerNum);
		}

		thisShip.shipController = ShipController.player;

		ship.tag = "Player Ship";
		ship.name = "playership" + playerNum;

		NetworkServer.Spawn (ship);
	}

	void GetAvailablePlayerShipList()
    {
		if (shipList == null || shipList.Length == 0)
        {
			shipList = new GameObject[10];

			for (int i = 0; i < 10; i++)
			{
				shipList[i] = (GameObject)Resources.Load("Prefabs/Ships/Ship_" + (i + 1), typeof(GameObject));
			}
		}
    }

	void AssignShip() {
		
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player Ship");

		if (players != null)
		{
			for (int i = 0; i < players.Length; i++)
			{
				Ship playerShip = players[i].GetComponent<Ship> ();
				if (playerShip.shipController != ShipController.player) 
				{
					continue;
				}

				Owner owner = players[i].GetComponent<Owner> ();
				if (owner) 
				{
					if (owner.GetOwnerNum() == playerNum) 
					{
						ship = players[i];
						break;
					}
				}
			}			
		}
	}

	void SetCameraToFollow() {
		ShipCamera sc = gameObject.GetComponent<ShipCamera> ();
		if (ship != null) {
			Ship myShip = ship.GetComponent<Ship> ();
			if (sc != null) {
				sc.SetTarget (myShip.gameObject);
			}
		}
	}

	public Ship GetShip() {
		if (ship != null) {
			return ship.GetComponent<Ship> ();
		} else {
			return null;
		}
	}

	public static Player GetLocalPlayer() {
		return thisPlayer;
	}

	public int GetPlayerNum() {
		return playerNum;
	}

	public void AddKill() {
		kills++;
	}
}
