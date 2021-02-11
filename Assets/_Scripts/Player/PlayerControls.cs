using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
struct CurrentShipAction
{
	public bool shooting;
	public bool turningLeft;
	public bool turningRight;
	public bool shield;
	public bool speeding;
	public bool slowing;
}

public class PlayerControls : NetworkBehaviour
{

	MouseControl mouseControl;
	Player player;
	CurrentShipAction currentAction;        //Use these as flags for holding down buttons so that commands aren't repeatedly sent.
	bool showChatBox;

	private void Start()
    {
		mouseControl = gameObject.AddComponent<MouseControl>();
	}

	public void AssignPlayer(Player player)
    {
		this.player = player;
    }

	[Client]
    public void CheckControls()
	{
		if (player == null)
		{
			player = GetComponent<Player>();
			
			if (player == null)
            {
				return;
            }
		}

		Ship ship = player.GetShip();
		BulletShooter shooter = null;
		if (ship)
		{
			shooter = ship.GetComponent<BulletShooter>();
		}

		if (shooter == null || ship == null)
        {
			return;
        }

		if (mouseControl.button[0])
		{
			//A little fancy math to get the angles to line up.
			float clickAngle = 180 - Angle.GetAngle(
				mouseControl.pos,
				new Vector2(Screen.width / 2, Screen.height / 2)
			);

			if (clickAngle < 0)
			{
				clickAngle += 360;
			}
		}

		Joystick ctr = GetComponent<Joystick>();
		if (ctr == null)
		{
			ctr = gameObject.AddComponent<Joystick>();
			//ctr.SetJoystick (1);
		}

		JoystickButton[] buttons = ctr.GetButtons();
		JoystickButton[] leftAxis = ctr.GetAxisHeld(0);

		if (!ctr.GetButtonsReady())
		{
			return;
		}

		int controlStyle = 0;

		float keyboardHorizontal = Input.GetAxis("Horizontal");
		float keyboardVertical = Input.GetAxis("Vertical");
		bool shootButton = Input.GetButton("Shoot");

		if (controlStyle == 0)
		{

			//Switch Weapons
			for (int i = 0; i < WeaponInfo.GetWeaponCount(); i++)
			{
				string which = "";
				if (i <= 9)
				{
					which = i.ToString();
				}
				else
				{
					switch (i)
					{
						case 10: which = "u"; break;
						case 11: which = "i"; break;
						case 12: which = "o"; break;
							//case 13: which = "p"; break;
					}
				}

				if (Input.GetKeyDown(which))
				{
					CmdChangeWeapon(ship, i);
				}
			}

			int currentWeapon = shooter.GetCurrentWeapon();
			int newCurrentWeapon = -1;

			int left = (int)AxisDir.Left;
			int right = (int)AxisDir.Right;
			int up = (int)AxisDir.Up;
			int down = (int)AxisDir.Down;

			if (leftAxis[left].IsHeld() && !leftAxis[left].IsChecked())
			{
				leftAxis[left].Check();
				switch (currentWeapon)
				{
					case 0: newCurrentWeapon = 1; break;    //Rockets
					case 1: newCurrentWeapon = 2; break;    //Missile
					case 2: newCurrentWeapon = 0; break;    //Machine Gun
					default: newCurrentWeapon = 0; break;
				}
			}
			else if (leftAxis[right].IsHeld() && !leftAxis[right].IsChecked())
			{
				leftAxis[right].Check();
				switch (currentWeapon)
				{
					case 3: newCurrentWeapon = 4; break;    //Crush
					case 4: newCurrentWeapon = 5; break;    //Nuke
					case 5: newCurrentWeapon = 3; break;    //Blaster
					default: newCurrentWeapon = 3; break;
				}
			}
			else if (leftAxis[up].IsHeld() && !leftAxis[up].IsChecked())
			{
				leftAxis[up].Check();
				switch (currentWeapon)
				{
					case 6: newCurrentWeapon = 7; break;    //Plasma
					case 7: newCurrentWeapon = 8; break;    //Mines
					case 8: newCurrentWeapon = 6; break;    //Warp
					default: newCurrentWeapon = 6; break;
				}
			}
			else if (leftAxis[down].IsHeld() && !leftAxis[down].IsChecked())
			{
				leftAxis[down].Check();
				switch (currentWeapon)
				{
					case 9: newCurrentWeapon = 10; break;   //Turret
					case 10: newCurrentWeapon = 11; break;  //Deactivator
					case 11: newCurrentWeapon = 9; break;   //Decoy
					default: newCurrentWeapon = 9; break;
				}
			}

			if (newCurrentWeapon >= 0)
			{
				CmdChangeWeapon(ship, newCurrentWeapon);
			}

			//Quit
			if (Input.GetKeyDown("escape"))
			{
				Application.Quit();
			}

			//Show Chat
			if (Input.GetKeyDown("t"))
			{
				showChatBox = true;
			}

			//Sent Chat
			if (Input.GetKeyDown("enter"))
			{
				showChatBox = false;
			}

			if (Input.GetKeyDown(KeyCode.X))
            {
				ship.GetComponent<Damageable>().Destruct();
            }

			//Shoot
			if (
					buttons[1].IsHeld() ||
					shootButton ||
					mouseControl.button[1]
				)
			{
				//CmdShootBullet();
				if (!currentAction.shooting)
				{
					currentAction.shooting = true;
					CmdSetShipToShoot(ship, true);
				}
			}
			else
			{
				if (currentAction.shooting)
				{
					currentAction.shooting = false;
					CmdSetShipToShoot(ship, false);
				}
			}

			//Shield
			if (
					buttons[2].IsHeld() ||
					Input.GetKey("z") ||
					mouseControl.button[3]
				)
			{
				if (!currentAction.shield)
				{
					currentAction.shield = true;
					CmdActivateShield(ship, true);
				}
			}
			else
			{
				if (currentAction.shield)
				{
					currentAction.shield = false;
					CmdActivateShield(ship, false);
				}
			}

			//Slow down
			if (
					buttons[4].IsHeld() ||
					keyboardVertical < 0
				)
			{
				if (!currentAction.slowing)
				{
					currentAction.slowing = true;
					CmdSlowShip(ship, true);
				}
			}
			else
			{
				if (currentAction.slowing)
				{
					currentAction.slowing = false;
					CmdSlowShip(ship, false);
				}
			}

			//Speed up
			if (
					buttons[5].IsHeld() ||
					keyboardVertical > 0
				)
			{
				if (!currentAction.speeding)
				{
					currentAction.speeding = true;
					CmdSpeedUpShip(ship, true);					
				}
			}
			else
			{
				if (currentAction.speeding)
				{
					currentAction.speeding = false;
					CmdSpeedUpShip(ship, false);
				}
			}

			//Turn Left
			if (
					buttons[6].IsHeld() ||
					keyboardHorizontal < 0
				)
			{
				if (!currentAction.turningLeft)
				{
					currentAction.turningLeft = true;
					CmdTurnShip(ship, 1);
				}
			}
			else
			{
				if (currentAction.turningLeft)
				{
					currentAction.turningLeft = false;

					CmdTurnShip(ship, 0);
				}
			}

			//Turn Right
			if (
					buttons[7].IsHeld() ||
					keyboardHorizontal > 0
				)
			{
				if (!currentAction.turningRight)
				{
					currentAction.turningRight = true;
					CmdTurnShip(ship, -1);
				}
			}
			else
			{
				if (currentAction.turningRight)
				{
					currentAction.turningRight = false;
					CmdTurnShip(ship, 0);
				}
			}

			//Dump Ammo
			if (
					buttons[8].GetTime() >= 2
				)
			{
				shooter.DumpAmmo(currentWeapon);
				shooter.WeaponsWithAmmoCount();
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

		for (int b = 0; b < buttons.Length; b++)
		{
			if (buttons[b].IsHeld())
			{
				buttons[b].Check();
			}
		}
	}

	[Command]
	void CmdChangeWeapon(Ship ship, int which)
	{
		if (ship != null)
		{
			BulletShooter shooter = ship.GetComponent<BulletShooter>();
			if (shooter)
			{
				shooter.SetCurrentWeapon(which);
			}
		}
	}

	[Command]	
	void CmdSpeedUpShip(Ship ship, bool speedingUp)
	{
		if (ship != null)
		{
			ship.Accel(speedingUp);
		}
	}

	[Command]
	void CmdSlowShip(Ship ship, bool slowingDown)
	{
		if (ship != null)
		{
			ship.Decel(slowingDown);
		}
	}

	[Command]
	void CmdTurnShip(Ship ship, int dir)
	{		
		if (ship != null)
		{
			ship.SetTurnDir(dir);
		}
	}

	[Command]
	void CmdActivateShield(Ship ship, bool setting)
	{
		if (ship != null)
		{
			ship.SetShield(setting);
		}
	}

	[Command]
	void CmdSetShipToShoot(Ship ship, bool isShooting)
	{
		if (ship != null)
		{
			BulletShooter shooter = ship.GetComponent<BulletShooter>();
			if (shooter)
			{
				shooter.SetIsFiring(isShooting);
			}
		}
	}

}
