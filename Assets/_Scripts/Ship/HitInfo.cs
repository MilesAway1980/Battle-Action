using UnityEngine;
using System.Collections;
using System;
public class HitInfo : MonoBehaviour
{

	bool freshKill;
	Guid lastHitBy;

	public void SetFreshKill()
	{
		freshKill = true;
	}

	public bool GetFreshKill()
	{
		bool value = freshKill;
		freshKill = false;
		return value;
	}

	public void SetLastHitBy(GameObject who)
	{

		Owner whoOwner = who.GetComponent<Owner>();
		if (whoOwner)
		{
			lastHitBy = whoOwner.GetOwnerGuid();
		}
		else
		{
			lastHitBy = Guid.Empty;
		}
	}

	public void SetLastHitBy(Guid who)
    {
		lastHitBy = who;
    }

	public Guid GetLastHitBy()
	{
		return lastHitBy;
	}
}
