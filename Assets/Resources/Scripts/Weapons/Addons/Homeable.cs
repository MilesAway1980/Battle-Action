using UnityEngine;
using System.Collections;

public class Homeable : MonoBehaviour {

	bool isHomeable;

	// Use this for initialization
	void Start () {
		isHomeable = true;
	}
	
	public bool getIsHomeable() {
		return isHomeable;
	}

	public void setIsHomeable(bool value) {
		isHomeable = value;
	}
}
