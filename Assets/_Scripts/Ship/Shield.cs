using UnityEngine;
using Mirror;
using System.Collections;

public class Shield : NetworkBehaviour
{

	//public float shieldHealth;
	//float currentHealth;
	public float shieldContactDamage;

	public float maxCharge;
	public float rechargeRate;
	[Range(1, 100)]
	public int damageReduction;
	float whenActivated;

	float charge;
	[SyncVar] bool active;

	GameObject shieldObject;
	//Ship owner;
	GameObject owner;

	// Use this for initialization
	void Start()
	{

		shieldObject = (GameObject)Instantiate(Resources.Load("Prefabs/Shield"));
		//owner = GetComponent<Ship> ();
		owner = gameObject;

		shieldObject.transform.parent = owner.transform;
		shieldObject.transform.position = transform.position;

		shieldObject.name = "My Shield";
		shieldObject.tag = "Shield";
		shieldObject.SetActive(false);

		charge = maxCharge;
	}

	void OnDestroy()
	{
		Destroy(shieldObject);
	}

	// Update is called once per frame
	void Update()
	{
		if (!active)
		{
			charge += rechargeRate;
			if (charge > maxCharge)
			{
				charge = maxCharge;
			}
		}
		else
		{
			charge -= 1;
			if (charge <= 0)
			{
				charge = 0;
				active = false;
			}
		}

		shieldObject.SetActive(active);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (!isServer)
        {
			return;
        }

		GameObject objectHit = col.gameObject;

		Damageable dm = objectHit.GetComponent<Damageable>();
		if (dm)
		{
			dm.Damage(shieldContactDamage);

			HitInfo info = objectHit.GetComponent<HitInfo>();
			if (info)
			{

				info.SetLastHitBy(owner);
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

	public void DamageShield(float amount)
	{
		charge -= amount;
		if (charge <= 0)
		{
			charge = 0;
		}
	}

	public int GetDamageReduction()
	{
		return damageReduction;
	}

	public float GetCharge()
	{
		return charge;
	}

	public float GetMaxCharge()
	{
		return maxCharge;
	}

	public void SetActive(bool setting)
	{
		active = setting;
	}

	public bool GetActive()
	{
		return active;
	}
}
