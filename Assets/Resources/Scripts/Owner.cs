using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Owner : NetworkBehaviour {

	[SyncVar] int ownerNum;
	[SyncVar] int numDecoy;

	public void setOwnerNum(int newOwnerNum) {
		ownerNum = newOwnerNum;
	}

	public int getOwnerNum() {
		return ownerNum;
	}

	public void addDecoy() {		
		numDecoy++;
	}

	public void removeDecoy() {		
		numDecoy--;
	}

	public int getNumDecoy() {
		return numDecoy;
	}

	public void emptyDecoy() {
		numDecoy = 0;
	}
}
