using UnityEngine;
using System.Collections;

public static class Intersect
{

	public static bool LineCircle(Vector2 lineStart, Vector2 lineEnd, Vector2 targetPos, float targetRadius, bool infinite = true)
	{
		float A = targetPos.x - lineStart.x;
		float B = targetPos.y - lineStart.y;
		float C = lineEnd.x - lineStart.x;
		float D = lineEnd.y - lineStart.y;

		if (!infinite)
		{

			Vector2 closest;

			/*
			float dot = A * C + B * D;
			float lineLen = C * C + D * D;
			float X = dot / lineLen;
			*/

			float X = (A * C + B * D) / (C * C + D * D);

			if (X < 0)
			{
				closest = lineStart;
			}
			else if (X > 1)
			{
				closest = lineEnd;
			}
			else
			{
				closest.x = lineStart.x + X * C;
				closest.y = lineStart.y + X * D;
			}

			if (Vector2.Distance(closest, targetPos) < targetRadius)
			{
				return true;
			}

		}
		else
		{
			/*
			float lineLen = Mathf.Sqrt(C * C + D * D);
			float dist = Mathf.Abs(A * D - B * C) / lineLen;
			*/

			float dist = Mathf.Abs(A * D - B * C) / Mathf.Sqrt(C * C + D * D);

			if (dist < targetRadius)
			{
				return true;
			}
		}

		return false;
	}
}
