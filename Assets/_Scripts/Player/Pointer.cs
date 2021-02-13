using UnityEngine;
using System.Collections;
using System;

public enum PointerType
{
	Player,
	Beacon,
	PowerUp
}

public class Pointer : MonoBehaviour
{
	static GameObject pointerHolder;
	static GameObject pointerObject;

	float pointerSize;
	float pointerDistance;

	GameObject pointerOwner;

	PointerType pointerType;	

	bool active;
	
	SpriteRenderer sr;

    void Update()
    {
		sr.enabled = active;
    }

	public static Pointer CreateNewPointer(PointerType pointerType, GameObject owner, float size, float distance)
	{
		if (pointerObject == null)
		{
			LoadPointerPrefab();
		}

		GameObject newPointerObject = Instantiate(pointerObject);
		newPointerObject.name = $"{GetPointerTypeName(pointerType)} Pointer";
		newPointerObject.transform.parent = pointerHolder.transform;
		newPointerObject.transform.localScale = new Vector3(size, size, size);

		Pointer newPointer = newPointerObject.GetComponent<Pointer>();

		newPointer.SetPointerType(pointerType);
		newPointer.SetSize(size);
		newPointer.SetDistance(distance);
		newPointer.SetOwner(owner);

		return newPointer;
	}

	Vector2 FindClosestBeacon()
    {
		ObjectList beaconList = Beacon.beaconList;
		GameObject closestBeacon = beaconList.GetClosest(pointerOwner);

		if (closestBeacon)
        {
			return closestBeacon.transform.position;
		}
		else
        {
			return Vector2.zero;
        }
	}

	Vector2 FindClosestPowerUp()
    {
		ObjectList powerupList = Powerup.powerupList;
		GameObject closestPowerup = powerupList.GetClosest(pointerOwner);

		if (closestPowerup)
        {
			return closestPowerup.transform.position;
        }
		else
        {
			return Vector2.zero;
        }
	}

	public Vector2 UpdatePointer()
    {
		Vector2 target = Vector2.zero;
		switch (pointerType)
		{
			case PointerType.Beacon: target = FindClosestBeacon(); break;
			case PointerType.PowerUp: target = FindClosestPowerUp(); break;
		}

		return UpdatePointer(target);
	}

	public Vector2 UpdatePointer(Vector2 target)
    {
		float angleToTarget = Angle.GetAngle(pointerOwner.transform.position, target, false);
		transform.rotation = Quaternion.Euler(0, 0, angleToTarget);

		float angleRad = angleToTarget * Mathf.Deg2Rad;

		transform.position = new Vector2(
			pointerOwner.transform.position.x - Mathf.Sin(angleRad) * pointerDistance,
			pointerOwner.transform.position.y + Mathf.Cos(angleRad) * pointerDistance
		);

		return target;
    }

	public PointerType GetPointerType()
    {
		return pointerType;
    }

	public void SetActive(bool activeSetting)
	{
		active = activeSetting;
	}

	public void SetPointerType(PointerType pointerType)
    {
		this.pointerType = pointerType;

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
		for (int i = 0; i < renderers.Length; i++)
		{
			/*
				"player" == "player"
				"beacon" == "beacon"
				"powerup" == "powerup"
			*/
			
			string nameOfRenderer = renderers[i].name;
			string nameOfPointerType = GetPointerTypeName(pointerType);

			if (nameOfRenderer.ToLower() == nameOfPointerType.ToLower())
            {
				sr = renderers[i];
				renderers[i].enabled = true;
            }
			else
            {
				renderers[i].enabled = false;
            }
		}
	}

	public static string GetPointerTypeName(PointerType pointerType)
    {
		int whichPointer = (int)pointerType;
		return Enum.GetName(typeof(PointerType), whichPointer);
	}

	public void SetSize(float size)
    {
		pointerSize = size;		
    }

	public void SetDistance(float distance)
    {
		pointerDistance = distance;
    }

	public void SetOwner(GameObject owner)
    {
		pointerOwner = owner;
    }	

	static void LoadPointerPrefab()
	{
		if (pointerHolder == null)
		{
			pointerHolder = new GameObject()
			{
				name = "Pointers"
			};
		}

		pointerObject = (GameObject)Resources.Load("Prefabs/Pointers/Pointer");
	}
}
