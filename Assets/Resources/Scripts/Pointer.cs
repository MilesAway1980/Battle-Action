﻿using UnityEngine;
using System.Collections;

public enum PointerType {
	player,
	beacon,
	powerUp
}

public class Pointer : MonoBehaviour {

	public float pointerSize;
	public float pointerDistance;
	public GameObject pointerObject;

	public PointerType pointerType;

	GameObject pointer;
	bool visible;
	bool active;

	SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		active = false;
	}

	// Update is called once per frame
	void Update () {

		if (active == false) {
			return;
		}

		if (pointer == null) {
			makePointer ();
		}

		ObjectInfo closestObject = new ObjectInfo();

		visible = true;

		float angle = 0;

		switch (pointerType) {
			case PointerType.player:
					
				ObjectList shipList = Ship.shipList;
				closestObject = shipList.getClosest (gameObject);

				visible = false;
				
				if ( 
						(
							closestObject.distance < ArenaInfo.getBeaconRange () ||
							closestObject.distance < ArenaInfo.getShipRadarRange ()
						) &&
						closestObject.distance >= 0
					) 
				{
					visible = true;
				}

				break;
		
			case PointerType.beacon:

				ObjectList beaconList = Beacon.beacons;
				closestObject = beaconList.getClosest (gameObject);

				break;
			case PointerType.powerUp:
				break;
		}

		if (visible) {
			angle = Angle.getAngle (transform.position, closestObject.pos);

			pointer.transform.position = new Vector3 (
				transform.position.x + Mathf.Sin (angle / Mathf.Rad2Deg) * pointerDistance,
				transform.position.y + Mathf.Cos (angle / Mathf.Rad2Deg) * pointerDistance,
				-5
			);

			pointer.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
			sr.enabled = true;
		} else {
			sr.enabled = false;
		}
	}

	void makePointer() {
		pointer = (GameObject)Instantiate (pointerObject);

		switch (pointerType) {
		case PointerType.player: pointer.name = "Player Pointer"; break;
		case PointerType.beacon: pointer.name = "Beacon Pointer"; break;
		case PointerType.powerUp: break;
		}

		pointer.transform.parent = transform;

		sr = pointer.GetComponent<SpriteRenderer> ();
		sr.transform.localScale = new Vector2 (pointerSize, pointerSize);
	}

	public void setActive(bool activeSetting) {
		active = activeSetting;
	}
}
