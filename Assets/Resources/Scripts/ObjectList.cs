﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectList {
	
	List<GameObject> objectList;
	
	public List<GameObject> getObjectList() {
		return objectList;
	}
	
	public void setObjectList(List<GameObject> newList) {
		objectList = newList;
	}

	public GameObject[] getObjects() {
		if (objectList == null) {
			return null;
		}
		GameObject[] allObjects = new GameObject[objectList.Count];
		for (int i = 0; i < objectList.Count; i++) {
			allObjects [i] = objectList [i];
		}

		return allObjects;
	}
	
	public void addObject(GameObject newObject) {
		if (objectList == null) {
			objectList = new List<GameObject>();
		}
		objectList.Add (newObject);
	}
	
	public void removeObject(GameObject deadObject) {
		if (objectList != null) {
			objectList.Remove (deadObject);
		}
	}

	public void clearList() {
		if (objectList != null) {
			objectList.Clear ();
		}
	}
	
	public GameObject getClosest(GameObject looking) {
		GameObject closest = null;
		
		float distance = float.MaxValue;

		Vector2 lookingPos = looking.transform.position;
		Vector2 targetPos;

		Owner lookingOwner = looking.GetComponent<Owner> ();

		if (objectList != null) {
			for (int i = 0; i < objectList.Count; i++) {

				if (objectList [i] == null) {
					continue;
				}

				Owner owner = objectList[i].GetComponent<Owner> ();
				if (owner) {
					if (owner.getNumDecoy () > 0) {
						continue;
					}
				}

				if (owner && lookingOwner) {
					if (owner.getOwnerNum() == lookingOwner.getOwnerNum()) {
						continue;
					}
				} else {
					if (looking == objectList[i]) {
						continue;
					}
				}

				if (objectList[i] != looking && objectList[i] != null) {

					targetPos = objectList[i].transform.position;			
					
					float objectDist = Vector2.Distance(
						lookingPos, 
						targetPos
					);				
					
					if (objectDist < distance) {
						distance = objectDist;
						closest = objectList[i];
					}
				}
			}
		}
		
		return closest;
	}

	public GameObject getObjectByOwner(int ownerNum) {
		for (int i = 0; i < objectList.Count; i++) {
			Owner owner = objectList [i].GetComponent<Owner> ();
			if (owner) {
				if (owner.getOwnerNum () == ownerNum) {
					return objectList [i];
				}
			}
		}
		return null;
	}

	public void print() {
		for (int i = 0; i < objectList.Count; i++) {
			Debug.Log (objectList[i]);
		}
	}
}
