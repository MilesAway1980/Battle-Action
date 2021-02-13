using UnityEngine;
using System.Collections.Generic;
using System;
public class ObjectList
{

	List<GameObject> objectList = new List<GameObject>();
	public int Count;

	public List<GameObject> GetObjectList()
	{
		return objectList;
	}

	public void SetObjectList(List<GameObject> newList)
	{
		objectList = newList;
	}

	public void AddObject(GameObject newObject)
	{
		objectList.Add(newObject);
		Count = objectList.Count;
	}

	public void RemoveObject(GameObject deadObject)
	{
		objectList.Remove(deadObject);
		Count = objectList.Count;
	}

	public void ClearList()
	{
		objectList.Clear();
	}

	public GameObject GetClosest(GameObject looking, GameObject[] objectsToIgnore = null)
	{
		GameObject closest = null;

		float distance = float.MaxValue;

		Vector2 lookingPos = looking.transform.position;
		//Vector2 targetPos;

		//Owner lookingOwner = looking.GetComponent<Owner>();
		bool hasNullEntry = false;
		if (objectList != null)
		{
			GameObject[] objectArray = objectList.ToArray();		//faster
			for (int i = 0; i < objectArray.Length; i++)
			{
				if (objectArray[i] == null)
				{
					hasNullEntry = true;
					continue;
				}

				if (objectArray[i] == looking)
				{
					//Don't look at itself
					continue;
				}

				if (objectsToIgnore != null)
                {
					for (int o = 0; 0 < objectsToIgnore.Length; o++)
                    {
						if (objectArray[i] == objectsToIgnore[o])
                        {
							continue;
                        }
                    }
                }

				float objectDist = Vector2.Distance(
					lookingPos,
					objectArray[i].transform.position
				);

				if (objectDist < distance)
				{
					distance = objectDist;
					closest = objectArray[i];
				}
			}
		}

		if (hasNullEntry)
        {
			objectList.RemoveAll(o => o == null);
        }

		return closest;
	}

	public GameObject GetObjectByOwner(Guid ownerGuid)
	{
		for (int i = 0; i < objectList.Count; i++)
		{
			Owner owner = objectList[i].GetComponent<Owner>();
			if (owner)
			{
				if (owner.GetOwnerGuid() == ownerGuid)
				{
					return objectList[i];
				}
			}
		}
		return null;
	}

	public void Print()
	{
		for (int i = 0; i < objectList.Count; i++)
		{
			Debug.Log(objectList[i]);
		}
	}
}
