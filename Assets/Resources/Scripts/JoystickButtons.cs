using UnityEngine;
using System.Collections;

public class JoystickButtons {

	bool isHeld = false;		//Is the button down
	bool isChecked = false;		//Has it's condition been checked?
	float timePressed = 0;	//When was it pressed?

	public void setHeld(bool held) {
		isHeld = held;
		
		if (isHeld == false) {
			isChecked = false;
			timePressed = 0;
		} else {
			timePressed = Time.fixedTime;
		}
	}

	public void check() {
		isChecked = true;
	}

	public bool getChecked () {
		return isChecked;
	}

	public bool getHeld() {
		return isHeld;
	}

	public float getTime() {
		if (isHeld) {
			return (Time.fixedTime - timePressed);
		} else {
			return 0;
		}
	}
}
