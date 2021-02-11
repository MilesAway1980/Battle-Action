using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShipCamera : MonoBehaviour
{
	Ship targetShip;
	float height;

	StarField starField;

	// Use this for initialization
	void Awake()
	{
		starField = gameObject.AddComponent<StarField>();
	}

	//private void FixedUpdate()
	private void Update()
	{
		MoveStars();
	}

	public void UpdatePosition(Vector2 newPos)
	{
		//oldPos = currentPos;

		Camera.main.transform.position = new Vector3(
			newPos.x,
			newPos.y,
			-height
		);
	}

	public void SetHeight(float newHeight)
	{
		if (newHeight >= 0)
		{
			height = newHeight;
		}
	}

	public void ChangeHeight(float change)
	{
		height += change;
		if (height < 0)
		{
			height = 0;
		}
	}

	public void SetTargetShip(Ship targetShip)
	{
		this.targetShip = targetShip;
	}

	void MoveStars()
	{
		//if (starField != null && targetShip != null) {
		if (starField != null && targetShip != null)
		{
			starField.MoveStars(
				targetShip.transform.position,
				targetShip.GetAngle(),
				targetShip.GetSpeed()
			);
		}
	}
}
