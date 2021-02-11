using UnityEngine;
using System.Collections;

public enum PointerType
{
	player,
	beacon,
	powerUp
}

public class Pointer : MonoBehaviour
{

	public float pointerSize;
	public float pointerDistance;
	public GameObject pointerObject;

	public PointerType pointerType;

	GameObject pointer;
	bool visible;
	bool active;

	SpriteRenderer sr;

	void Start()
	{

		active = false;
	}

	void OnDestroy()
	{
		Destroy(pointer);
	}

	void Update()
	{

		if (active == false)
		{
			return;
		}

		if (pointer == null)
		{
			MakePointer();
		}

		GameObject closestObject = null;
		GameObject closestBeacon = null;        //Beacons are used for all pointer types.

		visible = true;

		float angle = 0;

		ObjectList beaconList = Beacon.beaconList;
		ObjectList shipList = Ship.shipList;
		ObjectList powerupList = Powerup.powerupList;

		closestBeacon = beaconList.GetClosest(gameObject);

		switch (pointerType)
		{
			case PointerType.player:
				closestObject = shipList.GetClosest(gameObject);

				if (closestObject)
				{

					visible = false;

					//print (closestObject + "  " + closestObject.distance + " " + ArenaInfo.getBeaconRange () + " " + ArenaInfo.getShipRadarRange ());

					float beaconDist = Vector2.Distance(closestBeacon.transform.position, transform.position);
					float objectDist = Vector2.Distance(closestObject.transform.position, transform.position);

					if (
						(
							beaconDist < ArenaInfo.GetBeaconRange() || //A beacon is within range
							objectDist < ArenaInfo.GetShipRadarRange()          //Or the player is within range
						) && objectDist > 0                                     //Distance 0 == self.  Distance -1 == no object found.
					)
					{
						visible = true;
					}
				}

				break;

			case PointerType.beacon:

				closestObject = closestBeacon;

				break;
			case PointerType.powerUp:

				closestObject = powerupList.GetClosest(gameObject);
				if (closestObject)
				{
					visible = false;

					float beaconDist = Vector2.Distance(closestBeacon.transform.position, transform.position);
					float objectDist = Vector2.Distance(closestObject.transform.position, transform.position);

					if (
						(
							beaconDist < ArenaInfo.GetBeaconRange() || //A beacon is within range
							objectDist < ArenaInfo.GetShipRadarRange()          //Or the player is within range
						) && objectDist > 0                                     //Distance 0 == self.  Distance -1 == no object found.
					)
					{
						visible = true;
					}
				}
				break;
		}

		if (visible && closestObject)
		{
			angle = Angle.GetAngle(transform.position, closestObject.transform.position);
			float angleRad = angle * Mathf.Deg2Rad;

			pointer.transform.position = new Vector3(
				transform.position.x + Mathf.Sin(angleRad) * pointerDistance,
				transform.position.y + Mathf.Cos(angleRad) * pointerDistance
			);

			pointer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
			sr.enabled = true;
		}
		else
		{
			sr.enabled = false;
		}
	}

	void MakePointer()
	{
		pointer = Instantiate(pointerObject);

		switch (pointerType)
		{
			case PointerType.player: pointer.name = "Player Pointer"; break;
			case PointerType.beacon: pointer.name = "Beacon Pointer"; break;
			case PointerType.powerUp: pointer.name = "Powerup Pointer"; break;
		}

		pointer.tag = "Pointer";

		pointer.transform.parent = transform;

		sr = pointer.GetComponent<SpriteRenderer>();
		sr.transform.localScale = new Vector2(pointerSize, pointerSize);

	}

	public void SetActive(bool activeSetting)
	{
		active = activeSetting;
	}

	public GameObject GetPointer()
	{
		return pointer;
	}
}
