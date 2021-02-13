using UnityEngine;
using Mirror;
using System.Collections;

public class Shield : NetworkBehaviour
{
	public float shieldContactDamage;
	public float maxCharge;
	public float rechargeRate;

	[Range(1, 100)]
	public int damageReduction;

	float charge;
	[SyncVar] bool active;

	GameObject shieldObject;
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

	void OnCollisionStay2D(Collision2D collision)
	{
		if (!isServer)
        {
			return;
        }

		if (!active)
        {
			return;
        }

		Damageable dm = collision.gameObject.GetComponent<Damageable>();
		if (dm)
		{
			Owner shieldOwner = owner.GetComponent<Owner>();
			Owner hitOwner = collision.gameObject.GetComponent<Owner>();

			float damage = Random.Range(0, shieldContactDamage);

			if (shieldOwner && hitOwner)
            {
				if (shieldOwner.GetOwnerGuid() != hitOwner.GetOwnerGuid())
				{
					HitInfo info = collision.gameObject.GetComponent<HitInfo>();
					if (info)
					{
						info.SetLastHitBy(owner);
					}
				}
			}
            else
            {
				dm.Damage(damage);
            }
		}
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
