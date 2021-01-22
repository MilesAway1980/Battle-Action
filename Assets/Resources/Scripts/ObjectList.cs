using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectList {
	
	List<GameObject> objectList;
	
	public List<GameObject> GetObjectList() {
		return objectList;
	}
	
	public void SetObjectList(List<GameObject> newList) {
		objectList = newList;
	}

	public GameObject[] GetObjects() {
		if (objectList == null) {
			return null;
		}
		GameObject[] allObjects = new GameObject[objectList.Count];
		for (int i = 0; i < objectList.Count; i++) {
			allObjects [i] = objectList [i];
		}

		return allObjects;
	}
	
	public void AddObject(GameObject newObject) {
		if (objectList == null) {
			objectList = new List<GameObject>();
		}
		objectList.Add (newObject);
	}
	
	public void RemoveObject(GameObject deadObject) {
		if (objectList != null) {
			objectList.Remove (deadObject);
		}
	}

	public void ClearList() {
		if (objectList != null) {
			objectList.Clear ();
		}
	}
	
	public GameObject GetClosest(GameObject looking) {
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
					if (owner.GetNumDecoy() > 0) {	//Ship has a decoy, it can't be detected
						continue;
					}
				}

				if (owner && lookingOwner) {		//The one looking should see themselves
					if (owner.GetOwnerNum() == lookingOwner.GetOwnerNum()) {
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

	public GameObject GetObjectByOwner(int ownerNum) {
		for (int i = 0; i < objectList.Count; i++) {
			Owner owner = objectList [i].GetComponent<Owner> ();
			if (owner) {
				if (owner.GetOwnerNum () == ownerNum) {
					return objectList [i];
				}
			}
		}
		return null;
	}

	public void Print() {
		for (int i = 0; i < objectList.Count; i++) {
			Debug.Log (objectList[i]);
		}
	}
}
