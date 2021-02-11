using UnityEngine;
using Mirror;
using System;
using System.Collections;

public class Owner : NetworkBehaviour
{

	[SyncVar] Guid ownerGuid;
	[SyncVar] int numDecoy;
	bool isDecoy;

	public void SetOwnerGuid(Guid newOwnerGuid)
	{
		ownerGuid = newOwnerGuid;
	}

	public Guid GetOwnerGuid()
	{
		return ownerGuid;
	}

	public void AddDecoy()
	{
		numDecoy++;
	}

	public void RemoveDecoy()
	{
		numDecoy--;
	}

	public int GetNumDecoy()
	{
		return numDecoy;
	}

	public void EmptyDecoy()
	{
		numDecoy = 0;
	}

	public static Owner FindOwnerByGuid(Guid guid)
    {
		Owner[] owners = FindObjectsOfType<Owner>();
		for (int i = 0; i < owners.Length; i++)
        {
			if (owners[i].GetOwnerGuid() == guid && !owners[i].IsDecoy())
            {
				return owners[i];
            }
        }

		return null;
    }

	public bool IsDecoy()
    {
		return isDecoy;
    }

	public void SetDecoy(bool isDecoy)
    {
		this.isDecoy = isDecoy;
    }
}
