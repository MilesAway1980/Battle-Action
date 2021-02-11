using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum AxisDir
{
	Up,
	Left,
	Down,
	Right
}

public class Joystick : MonoBehaviour
{

	int whichJoystick = 1;
	//string joystickName;

	float[,] axis = new float[2, 2];
	JoystickButton[] buttons = new JoystickButton[12];
	JoystickButton[,] axisButton = new JoystickButton[2, 4];

	/*
	 *		0 = Left axis
	 *		1 = Right axis
	 *		
	 *		0 = up
	 *		1 = right
	 *		2 = down
	 *		3 = left
	 */

	string overlay = "";

	bool buttonsReady = false;
	float deadZone = 0.1f;

	void OnGUI()
	{
		if (Debugging.DebugMode())
		{
			GUI.Label(new Rect(800, 10, 200, 300), overlay);
		}
	}

	void Start()
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons[i] = new JoystickButton();
		}
		//whichJoystick = GetJoystick.getJoystick ();
		//joystickName = GetJoystick.getName (whichJoystick);
		//print (joystickName);
	}

	// Update is called once per frame
	void Update()
	{

		Init();
		if (!buttonsReady)
		{
			return;
		}

		axis[0, 0] = Input.GetAxis("Joy" + whichJoystick + "_Analog0_Horizontal");
		axis[0, 1] = Input.GetAxis("Joy" + whichJoystick + "_Analog0_Vertical");
		axis[1, 0] = Input.GetAxis("Joy" + whichJoystick + "_Analog1_Horizontal");
		axis[1, 1] = Input.GetAxis("Joy" + whichJoystick + "_Analog1_Vertical");

		if (Mathf.Abs(axis[0, 0]) < deadZone) { axis[0, 0] = 0; }
		if (Mathf.Abs(axis[0, 1]) < deadZone) { axis[0, 1] = 0; }
		if (Mathf.Abs(axis[1, 0]) < deadZone) { axis[1, 0] = 0; }
		if (Mathf.Abs(axis[1, 1]) < deadZone) { axis[1, 1] = 0; }

		axisButton[0, (int)AxisDir.Left].SetHeld(axis[0, 0] < 0);
		axisButton[0, (int)AxisDir.Right].SetHeld(axis[0, 0] > 0);
		axisButton[0, (int)AxisDir.Up].SetHeld(axis[0, 1] < 0);
		axisButton[0, (int)AxisDir.Down].SetHeld(axis[0, 1] > 0);

		axisButton[1, (int)AxisDir.Left].SetHeld(axis[0, 0] < 0);
		axisButton[1, (int)AxisDir.Right].SetHeld(axis[0, 0] > 0);
		axisButton[1, (int)AxisDir.Up].SetHeld(axis[0, 1] < 0);
		axisButton[1, (int)AxisDir.Down].SetHeld(axis[0, 1] > 0);

		for (int i = 0; i < 12; i++)
		{
			if (Input.GetButtonDown("Joy" + whichJoystick + "_Button" + (i + 1)))
			{
				buttons[i].SetHeld(true);
			}

			if (Input.GetButtonUp("Joy" + whichJoystick + "_Button" + (i + 1)))
			{
				buttons[i].SetHeld(false);
			}
		}

		PrintButtons();
	}

	public void Init()
	{
		if (!buttonsReady)
		{
			if (buttons.Length != 0)
			{
				buttonsReady = true;
				for (int i = 0; i < buttons.Length; i++)
				{
					if (buttons[i] == null)
					{
						buttonsReady = false;
					}
				}
			}
		}

		if (axisButton[0, 0] == null)
		{
			for (int x = 0; x < axisButton.GetLength(0); x++)
			{
				for (int y = 0; y < axisButton.GetLength(1); y++)
				{
					axisButton[x, y] = new JoystickButton();
				}
			}
		}
	}

	public void SetJoystick(int which)
	{
		whichJoystick = which;
	}

	public int GetJoystick()
	{
		return whichJoystick;
	}

	public JoystickButton[] GetButtons()
	{
		return buttons;
	}

	public float[,] GetAxis()
	{
		return axis;
	}

	public float GetAxis(int x, int y)
	{
		if ((x == 0 || x == 1) && (y == 0 || y == 1))
		{
			return axis[x, y];
		}

		return 0;
	}

	public float[] GetAxis(int x)
	{
		float[] yAxis = new float[2];
		if (x == 0 || x == 1)
		{
			yAxis[0] = axis[x, 0];
			yAxis[1] = axis[x, 1];
		}

		return yAxis;
	}

	public JoystickButton[,] GetAxisHeld()
	{
		return axisButton;
	}

	public JoystickButton[] GetAxisHeld(int x)
	{
		JoystickButton[] yAxis = new JoystickButton[4];
		if (x == 0 || x == 1)
		{
			for (int i = 0; i < axisButton.GetLength(1); i++)
			{
				yAxis[i] = axisButton[x, i];
			}
		}

		return yAxis;
	}

	public JoystickButton GetAxisHeld(int x, int y)
	{
		if ((x == 0 || x == 1) && (y == 0 || y == 1))
		{
			return axisButton[x, y];
		}

		return null;
	}

	public bool GetButtonsReady()
	{
		return buttonsReady;
	}

	public void PrintButtons()
	{
		if (Debugging.DebugMode())
		{

			overlay = "";

			for (int i = 0; i < Input.GetJoystickNames().Length; i++)
			{
				overlay += Input.GetJoystickNames()[i];
			}
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					overlay += $"\nAxis ({i}, {j}): {axis[i, j]}";
				}
			}

			for (int i = 0; i < 12; i++)
			{
				overlay += $"\n {i}: {buttons[i].IsHeld()} - {buttons[i].IsChecked()} - {buttons[i].GetTime()}";
			}
		}
	}
}
