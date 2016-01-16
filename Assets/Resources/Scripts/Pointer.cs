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

	void Start () {
		
		active = false;
	}

	void OnDestroy() {
		Destroy (pointer);
	}

	void Update () {

		if (active == false) {
			return;
		}

		if (pointer == null) {
			makePointer ();
		}

		GameObject closestObject = null;
		GameObject closestBeacon = null; 		//Beacons are used for all pointer types.

		visible = true;

		float angle = 0;

		ObjectList beaconList = Beacon.beaconList;
		ObjectList shipList = Ship.shipList;
		//ObjectList powerupList = Powerup.powerupList;

		closestBeacon  = beaconList.getClosest (gameObject);

		switch (pointerType) {
			case PointerType.player:				
				closestObject = shipList.getClosest (gameObject);
				if (closestObject) {

				visible = false;

				//print (closestObject + "  " + closestObject.distance + " " + ArenaInfo.getBeaconRange () + " " + ArenaInfo.getShipRadarRange ());

				float beaconDist = Vector2.Distance (closestBeacon.transform.position, transform.position);
				float objectDist = Vector2.Distance (closestObject.transform.position, transform.position);
					
				if ( 
						(
						    beaconDist < ArenaInfo.getBeaconRange () || //A beacon is within range
						    objectDist < ArenaInfo.getShipRadarRange ()			//Or the player is within range
						) && objectDist > 0										//Distance 0 == self.  Distance -1 == no object found.
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
				break;
		}

		if (visible && closestObject) {
			angle = Angle.getAngle (transform.position, closestObject.transform.position);

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

		pointer.tag = "Pointer";


		pointer.transform.parent = transform;

		sr = pointer.GetComponent<SpriteRenderer> ();
		sr.transform.localScale = new Vector2 (pointerSize, pointerSize);

	}

	public void setActive(bool activeSetting) {
		active = activeSetting;
	}

	public GameObject getPointer() {
		return pointer;
	}
}
