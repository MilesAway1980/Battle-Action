using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour {

	public Vector2 pos;
	public Vector2 lastPos;
	public int x;
	public int y;
	public bool[] button;
	public float speed;

	public int wheel;

	private int wheelState;
	private int numButtons;

	void Start () {

		numButtons = 5;

		pos = Input.mousePosition;
		lastPos = pos;
		button = new bool[numButtons];
	}
	
	void Update () {
		//print (Input.mousePosition);
		lastPos = pos;
		pos = Input.mousePosition;
		x = (int)pos.x;
		y = (int)pos.y;

		for (int i = 0; i < button.Length; i++) {
			button [i] = Input.GetMouseButton (i);
		}
		speed = Vector2.Distance (pos, lastPos);

		wheel = (int)Input.mouseScrollDelta.y;
		if (wheel < 0) {
			wheelState = -1;
		} else if (wheel > 0) {
			wheelState = 1;
		}
	}

	public int getWheel(bool reset) {		
		if (reset) {
			int val = wheelState;
			wheelState = 0;
			return val;
		}

		return wheelState;
	}

	public void wheelReset() {
		wheelState = 0;
	}
}
