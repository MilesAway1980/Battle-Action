using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class Homing : NetworkBehaviour
{

	public float turnRate;
	public float detectDistance;

	Guid ownerGuid;
	GameObject target;

	void FixedUpdate()
	{

		if (isServer)
		{
			if (target == null)
			{
				GetTarget();
			}
			else
			{
				ChaseTarget();
			}
		}
	}

	[Server]
	void GetTarget()
	{
		List<GameObject> targets = Damageable.damageableList.GetObjectList();

		if (targets.Count == 0)
		{
			return;
		}

		float closest = float.MaxValue;
		for (int i = 0; i < targets.Count; i++)
		{
			GameObject potentialTarget = targets[i].gameObject;
			Owner targetOwner = potentialTarget.GetComponent<Owner>();
			if (targetOwner && targetOwner.GetOwnerGuid() == ownerGuid)
            {
				continue;
            }

			float distance = Vector2.Distance(potentialTarget.transform.position, transform.position);
			if (distance < closest && distance < detectDistance)
			{
				closest = distance;
				target = potentialTarget;
			}
		}
	}

	[Server]
	void ChaseTarget()
	{

		Bullet thisBullet = GetComponent<Bullet>();

		float currentAngle = transform.eulerAngles.z;
		float angleToTarget = 360 - Angle.GetAngle(transform.position, target.transform.position);

		int turnDir = Angle.GetDirection(currentAngle, angleToTarget);
		float angleChange = turnRate * turnDir;

		if (Mathf.Abs(currentAngle - angleToTarget) < Mathf.Abs(angleChange))
        {
			angleChange = angleToTarget - currentAngle;
        }

		thisBullet.ChangeAngle(angleChange);

		//Lose target if it gets out of range.
		if (Vector2.Distance(transform.position, target.transform.position) > detectDistance)
		{
			target = null;
		}
	}

	public void SetOwner(GameObject newOwner)
	{
		Owner owner = newOwner.GetComponent<Owner>();
		if (owner)
        {
			ownerGuid = owner.GetOwnerGuid();
        }
	}

	public void SetOwner(Guid ownerGuid)
    {
		this.ownerGuid = ownerGuid;
    }

	public float GetTurnRate()
	{
		return turnRate;
	}

	public void SetTurnRate(float newTurnRate)
	{
		if (newTurnRate >= 0)
		{
			turnRate = newTurnRate;
		}
	}
}
