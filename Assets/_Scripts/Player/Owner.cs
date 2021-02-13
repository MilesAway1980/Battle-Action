using UnityEngine;
using Mirror;
using System;
using System.Collections;

public class Owner : NetworkBehaviour
{

	[SyncVar] Guid ownerGuid;
	//[SyncVar] int numDecoy;
	bool isDecoy;

	public void SetOwnerGuid(Guid newOwnerGuid)
	{
		ownerGuid = newOwnerGuid;
	}

	public Guid GetOwnerGuid()
	{
		return ownerGuid;
	}

	public static Owner[] GetAllOwners()
    {
		return FindObjectsOfType<Owner>();
    }

	public static Owner FindOwnerByGuid(Guid guid, bool includeDecoy = false)
    {
		Owner[] owners = FindObjectsOfType<Owner>();
		for (int i = 0; i < owners.Length; i++)
        {
			if (owners[i].GetOwnerGuid() == guid && (includeDecoy || !owners[i].IsDecoy()))
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

	/*public bool HasDecoy()
    {
		print($"Has Decoy: {numDecoy}");
		return numDecoy > 0;
    }*/
}
