using UnityEngine;
using System.Collections;

public class HitInfo : MonoBehaviour {

	bool freshKill;
	int lastHitBy;

	public void setFreshKill() {
		freshKill = true;
	}

	public bool getFreshKill() {
		bool value = freshKill;
		freshKill = false;
		return value;
	}

	public void setLastHitBy(GameObject who) {

		Owner whoOwner = who.GetComponent<Owner> ();
		if (whoOwner) {
			lastHitBy = whoOwner.getOwnerNum ();
		} else {
			lastHitBy = -1;
		}
	}

	public int getLastHitBy() {
		return lastHitBy;
	}
}
