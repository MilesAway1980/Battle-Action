using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Controls : MonoBehaviour {

	int whichJoystick = 1;
	//string joystickName;

	float[,] axis = new float[2,2];
	JoystickButtons[] buttons = new JoystickButtons[12];

	string overlay = "";

	void OnGUI()
	{
		if (Debugging.debugMode()) {
			GUI.Label (new Rect (10, 10, 200, 300), overlay);
		}
	}

	void Start() {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i] = new JoystickButtons();
		}
		//whichJoystick = GetJoystick.getJoystick ();
		//joystickName = GetJoystick.getName (whichJoystick);
		//print (joystickName);
	}

	// Update is called once per frame
	void Update () {

		Rigidbody2D rb = this.GetComponent<Rigidbody2D> ();
		if (rb == null) {
			return;
		}

		axis[0,0] = Input.GetAxis ("Joy" + whichJoystick + "_Analog0_Horizontal");
		axis[0,1] = Input.GetAxis ("Joy" + whichJoystick + "_Analog0_Vertical");
		axis[1,0] = Input.GetAxis ("Joy" + whichJoystick + "_Analog1_Horizontal");
		axis[1,1] = Input.GetAxis ("Joy" + whichJoystick + "_Analog1_Vertical");

		for (int i = 0; i < 12; i++) {
			if (Input.GetButtonDown ("Joy" + whichJoystick + "_Button" + (i + 1)))
			{
				buttons[i].setHeld(true);
			}

			if (Input.GetButtonUp ("Joy" + whichJoystick + "_Button" + (i + 1)))
			{
				buttons[i].setHeld(false);
			}
		}

		printButtons ();
	}

	public void setJoystick(int which) {
		whichJoystick = which;
	}

	public int getJoystick() {
		return whichJoystick;
	}

	public JoystickButtons[] getButtons() {
		return buttons;
	}

	public float[,] getAxis() {
		return axis;
	}

	public void printButtons()
	{
		if (Debugging.debugMode()) {
			
			overlay = "";
			
			for (int i = 0; i < Input.GetJoystickNames().Length; i++)
			{
				overlay += Input.GetJoystickNames()[i];
			}
			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < 2; j++) {
					overlay += "\nAxis (" + i + ", " + j + "): " + axis[i, j];
				}
			}
			
			for (int i = 0; i < 12; i++) {
				overlay += "\n" + i + ": " + buttons[i].getHeld ();
				overlay += " - " + buttons[i].getChecked();
				overlay += " - " + buttons[i].getTime ();
			}		
		}
	}
}
