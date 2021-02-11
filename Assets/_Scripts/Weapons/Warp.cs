using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class Warp : NetworkBehaviour
{

	public float warpTimer;

	[Range(0, 100)]
	public float minPercentageOfArenaToWarp;
	[Range(0, 100)]
	public float maxPercentageOfArenaToWarp;
	public WarpField warpField;

	public float minimumWarpDistance;
	public float refireRate;
	public float warpTrailDamage;

	GameObject ownerObject;
	Guid ownerGuid;
	[SyncVar] float startTime;
	static float warpRefireRate = -1;
	[SyncVar] bool warped;

	// Use this for initialization
	void Start()
	{
		if (isServer)
		{
			startTime = Time.realtimeSinceStartup;
			warped = false;
			warpField.transform.localScale = Vector3.zero;

			Owner owner = ownerObject.GetComponent<Owner>();
			if (owner)
			{
				ownerGuid = owner.GetOwnerGuid();
			}
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{

		if (!isServer)
		{
			return;
		}

		if (warped == false && (Time.realtimeSinceStartup - startTime) >= warpTimer)
		{
			WarpShip();
		}
		else
		{
			if (ownerObject == null || warpField == null)
			{
				Destroy(gameObject);
			}
		}
	}

	[Server]
	void WarpShip()
	{
		float distance = UnityEngine.Random.Range(
			ArenaInfo.GetArenaSize() * 2 * (minPercentageOfArenaToWarp / 100.0f),
			ArenaInfo.GetArenaSize() * 2 * (maxPercentageOfArenaToWarp / 100.0f)
		);

		if (distance < minimumWarpDistance)
		{
			distance = minimumWarpDistance;
		}

		float angleDeg = ownerObject.transform.eulerAngles.z;
		float angleRad = angleDeg * Mathf.Deg2Rad;

		/*Vector2 halfWay = new Vector2(
			owner.transform.position.x - Mathf.Sin(angleRad) * (distance * 0.5f),
			owner.transform.position.y + Mathf.Cos(angleRad) * (distance * 0.5f)
		);*/



		Vector2 destination = new Vector3(
			ownerObject.transform.position.x - Mathf.Sin(angleRad) * distance,
			ownerObject.transform.position.y + Mathf.Cos(angleRad) * distance
		);

		Vector2 halfWay = Vector2.Lerp(ownerObject.transform.position, destination, 0.5f);

		float length = Vector2.Distance(ownerObject.transform.position, halfWay);

		ownerObject.transform.position = destination;
		
		warpField.Init(ownerObject.transform.eulerAngles.z, halfWay, length);
		warped = true;
	}

	void OnTriggerStay2D(Collider2D collider)
	{

		if (!isServer)
		{
			return;
		}

		Damageable dm = collider.gameObject.GetComponent<Damageable>();
		if (dm)
		{			
			Owner hitOwner = collider.gameObject.GetComponent<Owner>();
			if (hitOwner && hitOwner.GetOwnerGuid() != ownerGuid)
			{
				dm.Damage(warpTrailDamage);
				HitInfo info = collider.gameObject.GetComponent<HitInfo>();
				if (info)
				{
					info.SetLastHitBy(ownerGuid);
				}
			}
		}
	}

	public static GameObject GetWarp()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Warp");
	}

	public static float GetRefireRate()
	{
		if (warpRefireRate == -1)
		{
			warpRefireRate = GetWarp().GetComponent<Warp>().refireRate;
		}
		return warpRefireRate;
	}

	public void Init(GameObject ownerObject)
	{
		this.ownerObject = ownerObject;
	}
}
