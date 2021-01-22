using UnityEngine;
using System.Collections;

public class JoystickButtons {

	bool isHeld = false;		//Is the button down
	bool isChecked = false;		//Has it's condition been checked?
	float timePressed = 0;	//When was it pressed?

	public void SetHeld(bool held) {
		isHeld = held;
		
		if (isHeld == false) {
			isChecked = false;
			timePressed = 0;
		} else {
			timePressed = Time.fixedTime;
		}
	}

	public void Check() {
		isChecked = true;
	}

	public bool GetChecked () {
		return isChecked;
	}

	public bool GetHeld() {
		return isHeld;
	}

	public float GetTime() {
		if (isHeld) {
			return (Time.fixedTime - timePressed);
		} else {
			return 0;
		}
	}
}
