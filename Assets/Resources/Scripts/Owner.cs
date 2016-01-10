using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Owner : NetworkBehaviour {

	[SyncVar] int ownerNum;

	public void setOwnerNum(int newOwnerNum) {
		ownerNum = newOwnerNum;
	}

	public int getOwnerNum() {
		return ownerNum;
	}
}
