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
		if (Debugging.DebugMode()) {
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

		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
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
				buttons[i].SetHeld(true);
			}

			if (Input.GetButtonUp ("Joy" + whichJoystick + "_Button" + (i + 1)))
			{
				buttons[i].SetHeld(false);
			}
		}

		PrintButtons ();
	}

	public void SetJoystick(int which) {
		whichJoystick = which;
	}

	public int GetJoystick() {
		return whichJoystick;
	}

	public JoystickButtons[] GetButtons() {
		return buttons;
	}

	public float[,] GetAxis() {
		return axis;
	}

	public void PrintButtons()
	{
		if (Debugging.DebugMode()) {
			
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
				overlay += "\n" + i + ": " + buttons[i].GetHeld ();
				overlay += " - " + buttons[i].GetChecked();
				overlay += " - " + buttons[i].GetTime ();
			}		
		}
	}
}
