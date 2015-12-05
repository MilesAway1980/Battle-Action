using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Shield : NetworkBehaviour {

	//public float shieldHealth;
	//float currentHealth;
	public float shieldContactDamage;

	public float maxCharge;
	public float rechargeRate;
	float whenActivated;

	float charge;
	[SyncVar] bool active;

	GameObject shieldObject;
	Ship owner;

	// Use this for initialization
	void Start () {

		shieldObject = (GameObject)Instantiate(Resources.Load ("Prefabs/Shield"));
		owner = GetComponent<Ship> ();
		
		shieldObject.transform.parent = owner.transform;
		shieldObject.transform.position = transform.position;
		
		shieldObject.name = "my shield";

		setActive (false);

		charge = maxCharge;

	}
	
	// Update is called once per frame
	void Update () {
		if (!active) {
			charge += rechargeRate;
			if (charge > maxCharge) {
				charge = maxCharge;
			}
		} else {
			charge -= 1;
			if (charge <= 0) {
				charge = 0;
				active = false;
			}
		}

		shieldObject.SetActive (active);
	}

	void OnCollisionEnter2D(Collision2D col) {
		GameObject objectHit = col.gameObject;
		
		if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();
			shipHit.damage(shieldContactDamage);
			shipHit.setLastHitBy(owner.getOwner());
		}		
	}

	public float getCharge() {
		return charge;
	}

	public float getMaxCharge() {
		return maxCharge;
	}

	public void setActive(bool setting) {
		active = setting;
	}
}
