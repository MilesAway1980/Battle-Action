using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Shield : NetworkBehaviour {

	//public float shieldHealth;
	//float currentHealth;
	public float shieldContactDamage;

	public float maxCharge;
	public float rechargeRate;
	public float damageReduction;
	float whenActivated;

	float charge;
	[SyncVar] bool active;

	GameObject shieldObject;
	//Ship owner;
	GameObject owner;

	// Use this for initialization
	void Start () {

		shieldObject = (GameObject)Instantiate(Resources.Load ("Prefabs/Shield"));
		//owner = GetComponent<Ship> ();
		owner = this.gameObject;
		
		shieldObject.transform.parent = owner.transform;
		shieldObject.transform.position = transform.position;
		
		shieldObject.name = "my shield";
		shieldObject.tag = "Shield";

		setActive (false);

		charge = maxCharge;

	}

	void OnDestroy() {
		Destroy (shieldObject);
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

		Damageable dm = objectHit.GetComponent<Damageable> ();
		if (dm) {
			dm.damage (shieldContactDamage);

			HitInfo info = objectHit.GetComponent<HitInfo> ();
			if (info) {

				info.setLastHitBy (owner);					
			}

			/*Ship shipHit = objectHit.GetComponent<Ship> ();
			if (shipHit) {
				Owner thisOwner = owner.GetComponent<Owner> ();
				if (thisOwner) {
					shipHit.setLastHitBy (thisOwner.getOwnerNum ());
				} else {
					shipHit.setLastHitBy (-1);
				}
			}*/
		}


		
		/*if (objectHit.tag == "Player Ship") {
			Ship shipHit = objectHit.GetComponent<Ship>();

			if (shipHit) {
				shipHit.damage (shieldContactDamage);
				shipHit.setLastHitBy (owner.getOwnerNum ()); 
			}
		}*/		
	}

	public void damageShield(float amount) {
		charge -= amount;
		if (charge <= 0) {
			charge = 0;
		}
	}

	public float getDamageReduction() {
		return damageReduction;
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

	public bool getActive() {
		return active;
	}
}
