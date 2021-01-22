using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Star
{

	GameObject starObject;
	SpriteRenderer sr;
	Vector2 pos;

	float size;
	float speed;

	int maxXDistance = 11;
	int maxYDistance = 6;

	int starLayer = 0;

	public void Init(GameObject newSpriteObject, Vector2 newPos, Transform parent)
	{
		starObject = newSpriteObject;
		starObject.transform.position = newPos;
		starObject.transform.parent = parent;
		sr = starObject.GetComponent<SpriteRenderer>();
		newSpriteObject.layer = starLayer;

		ResetStar();

		sr.transform.localScale = new Vector2(size, size);

		pos = newPos;
	}

	public void Move(Vector2 camPos, float angle, float moveSpeed)
	{

		float radAngle = angle / Mathf.Rad2Deg;

		pos = new Vector2(
			pos.x - Mathf.Sin(radAngle) * (moveSpeed * speed),
			pos.y - Mathf.Cos(radAngle) * (moveSpeed * speed)
		);

		Vector2 screenCoord = new Vector2(camPos.x + pos.x, camPos.y + pos.y);

		if (screenCoord.x < (camPos.x - maxXDistance))
		{
			pos = new Vector2(pos.x + maxXDistance * 2, pos.y);
			ResetStar();
		}
		else if (screenCoord.x > (camPos.x + maxXDistance))
		{
			pos = new Vector2(pos.x - maxXDistance * 2, pos.y);
			ResetStar();
		}

		if (screenCoord.y < (camPos.y - maxYDistance))
		{
			pos = new Vector2(pos.x, pos.y + maxYDistance * 2);
			ResetStar();
		}
		else if (screenCoord.y > (camPos.y + maxYDistance))
		{
			pos = new Vector2(pos.x, pos.y - maxYDistance * 2);
			ResetStar();
		}

		starObject.transform.position = screenCoord;


	}

	public void SetStarLayer(int newLayer)
	{
		if (newLayer >= 0 && newLayer < 32)
		{
			starLayer = newLayer;
			starObject.layer = newLayer;
		}
	}

	void ResetStar()
	{
		size = (float)Random.Range(0.0f, 0.25f);
		speed = (float)Random.Range(0.0f, 0.25f);
	}


}