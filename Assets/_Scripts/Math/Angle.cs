using UnityEngine;
using System.Collections;


/*
			   Y 
  
			   +
			   +
			   +
	-X	---			+++		X
			   -		
			   -
			   -

			   -Y


	Starting Direction (0°)			Rotation Direction			  X			Y
	
			U								CW					 SIN		 COS
			U								CCW					-SIN		 COS		
			R								CW					 COS		-SIN		
			R								CCW					 COS		 SIN
			D								CW					-SIN		-COS		
			D								CCW					 SIN		-COS		
			L								CW					-COS		 SIN
			L								CCW					-COS		-SIN

 
 */





public static class Angle
{

	public static readonly float Pi = 3.1415926535897932384626433832795f;
	public static readonly float DoublePi = 6.283185307179586476925286766559f;

	public static float GetAngle(Vector2 start, Vector2 end, bool clockwise = true)
	{
		if (clockwise)
		{
			return (Mathf.Atan2(start.x - end.x, start.y - end.y)) * Mathf.Rad2Deg + 180;
		}
		else
		{
			float ang = (Mathf.Atan2(start.y - end.y, start.x - end.x)) * Mathf.Rad2Deg;

			ang = ang <= 180 && ang >= -90 
				? ang += 90 
				: ang += 450;			

			return ang;
		}
	}

	/*
	 * angleDist should always be >= 0 
	 * If it is -1, then it has not been supplied, and should be calculated.
	 * Otherwise, the caller may have already calculated it for other purposes
	 * and it should be reused to avoid calculating it twice.
	*/
	public static int GetDirection(float currentAngle, float targetAngle, float angleDist = -1)
	{

		if (targetAngle != currentAngle)
		{

			if (angleDist == -1)
			{
				angleDist = Mathf.Abs(targetAngle - currentAngle);
			}

			/*
			 * If the distance between the two angles is greater than 180*
			 * then the arc between the two values is larger than half the circle.
			 * For example:  currentAngle = 5° and targetAngle = 355°
			 * angleDist will be 350, even though these two angles are only 10°
			 * apart.  Calculations should be reversed for this scenario to keep the rotation
			 * from turning the full arc of the circle to point the correct direction.
			*/

			//Debug.Log (angleDist);

			if (angleDist > 180)
			{
				if (targetAngle > currentAngle)
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}
			else
			{
				if (targetAngle < currentAngle)
				{
					return -1;
				}
				else
				{
					return 1;
				}
			}
		}
		else
		{
			return 0;
		}
	}

	public static float FixAngle(float angle)
	{
		while (angle < 0)
		{
			angle += 360;
		}

		while (angle >= 360)
		{
			angle -= 360;
		}

		return angle;
	}
}

