using UnityEngine;
using System.Collections;

public class HitInfo : MonoBehaviour {

	bool freshKill;
	int lastHitBy;

	public void SetFreshKill() {
		freshKill = true;
	}

	public bool GetFreshKill() {
		bool value = freshKill;
		freshKill = false;
		return value;
	}

	public void SetLastHitBy(GameObject who) {

		Owner whoOwner = who.GetComponent<Owner> ();
		if (whoOwner) {
			lastHitBy = whoOwner.GetOwnerNum ();
		} else {
			lastHitBy = -1;
		}
	}

	public int GetLastHitBy() {
		return lastHitBy;
	}
}
