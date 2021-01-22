using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Owner : NetworkBehaviour {

	[SyncVar] int ownerNum;
	[SyncVar] int numDecoy;

	public void SetOwnerNum(int newOwnerNum) {
		ownerNum = newOwnerNum;
	}

	public int GetOwnerNum() {
		return ownerNum;
	}

	public void AddDecoy() {		
		numDecoy++;
	}

	public void RemoveDecoy() {		
		numDecoy--;
	}

	public int GetNumDecoy() {
		return numDecoy;
	}

	public void EmptyDecoy() {
		numDecoy = 0;
	}
}
