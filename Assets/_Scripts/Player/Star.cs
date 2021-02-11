using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Star
{
	GameObject starObject;

	Vector2 pos;

	float size;
	float speed;

	static readonly int maxXDistance = 12;
	static readonly int maxYDistance = 7;
	static int starDepth;
	static float maxSize = 0.25f;
	static float maxSpeed = 0.001f;

	public void Init(GameObject newSpriteObject)
	{
		ResetStar();

		starObject = newSpriteObject;
		SpriteRenderer spriteRenderer = starObject.GetComponent<SpriteRenderer>();
		spriteRenderer.transform.localScale = new Vector3(size, size);
		pos = starObject.transform.position;
	}

	public void Move(Vector2 camPos, float angle, float moveSpeed)
	{
		float radAngle = angle * Mathf.Deg2Rad;

		pos = new Vector2(
			pos.x + Mathf.Sin(radAngle) * (moveSpeed * speed),
			pos.y - Mathf.Cos(radAngle) * (moveSpeed * speed)
		);

		Vector3 screenCoord = new Vector3(camPos.x + pos.x, camPos.y + pos.y, starDepth);

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

	public static void SetStarDepth(int newDepth)
	{
		starDepth = newDepth;
	}

	public void SetStarLayer(int newLayer)
	{
		if (newLayer >= 0 && newLayer < 32)
		{
			starObject.layer = newLayer;
		}
	}

	void ResetStar()
	{
		size = Random.Range(0.0f, maxSize);
		speed = Random.Range(0.0f, maxSpeed);
	}
}