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

	public GameObject GetClosest(GameObject looking)
	{
		GameObject closest = null;

		float distance = float.MaxValue;

		Vector2 lookingPos = looking.transform.position;
		Vector2 targetPos;

		Owner lookingOwner = looking.GetComponent<Owner>();

		if (objectList != null)
		{
			for (int i = 0; i < objectList.Count; i++)
			{

				if (objectList[i] == null)
				{
					continue;
				}

				Owner owner = objectList[i].GetComponent<Owner>();
				if (owner)
				{
					if (owner.GetNumDecoy() > 0)
					{   //Ship has a decoy, it can't be detected
						continue;
					}
				}

				if (owner && lookingOwner)
				{       //The one looking should see themselves
					if (owner.GetOwnerGuid() == lookingOwner.GetOwnerGuid())
					{
						continue;
					}
				}
				else
				{
					if (looking == objectList[i])
					{
						continue;
					}
				}

				if (objectList[i] != looking && objectList[i] != null)
				{

					targetPos = objectList[i].transform.position;

					float objectDist = Vector2.Distance(
						lookingPos,
						targetPos
					);

					if (objectDist < distance)
					{
						distance = objectDist;
						closest = objectList[i];
					}
				}
			}
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
