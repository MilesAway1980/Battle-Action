using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class Plasma : Bullet {

	static ShootingInfo plasmaInfo;
	static Plasma basePlasma;
	
	public float maxChargeTime;
	public float initialCharge;
	public float chargeDistanceFromOwner;
	public float damageMultiplierPerSecondofCharge;

	static List<Plasma> plasmaList = new List<Plasma>();

	PlasmaSphere[] plasmaSpheres;
	PlasmaTrail[] plasmaTrails;

	[Range (0, 180)]
	public float arc;

	[SyncVar] float charge;
	[SyncVar] float radius;
	//bool released;

	bool charging = true;

	GameObject plasmaOwner;
	float originalAngle;

	// Use this for initialization
	void Start () {	

		if (ownerGuid == Guid.Empty)
        {
			return;
        }

		//released = false;
		charge = initialCharge;
		radius = 0;

		travelDist = ArenaInfo.GetArenaSize() * 1.25f;
		if (travelDist < ArenaInfo.GetMinBulletTravelDist()) {
			travelDist = ArenaInfo.GetMinBulletTravelDist();
		}

		plasmaSpheres = GetComponentsInChildren<PlasmaSphere>();
		plasmaTrails = GetComponentsInChildren<PlasmaTrail>();

		//Debug.Log(plasmaSpheres.Length);
		//Debug.Log(plasmaTrails.Length);

		plasmaList.Add(this);
		//Debug.Log(plasmaList.Count);

		originalAngle = plasmaOwner.transform.eulerAngles.z;
	}	

	void OnDestroy()
    {
		plasmaList.Remove(this);
    }	
	
	void FixedUpdate () {

		if (!isServer && ownerGuid == Guid.Empty)
        {
			return;
        }

		if (plasmaOwner == null)
		{
			charging = false;
		}

		if (charging)
		{
			float angleChange = plasmaOwner.transform.rotation.eulerAngles.z - originalAngle;
			angleRad = (transform.rotation.eulerAngles.z + angleChange) * Mathf.Deg2Rad;
			transform.position = new Vector2(
				plasmaOwner.transform.position.x - Mathf.Sin(angleRad) * chargeDistanceFromOwner,
				plasmaOwner.transform.position.y + Mathf.Cos(angleRad) * chargeDistanceFromOwner
			);
		}
		else
        {
			rigidBody.velocity = new Vector2(
				-Mathf.Sin(angleRad) * speed,
				Mathf.Cos(angleRad) * speed
			);
        }

		SetRadius(charge / 5.0f);		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (!isServer)
        {
			return;
        }

		Damageable dm = collision.gameObject.GetComponent<Damageable>();
		if (dm)
        {
			//float chargeDamage = (maxChargeTime + initialCharge - charge) * damageMultiplierPerSecondofCharge;
			float chargeDamage = (charge - initialCharge) * damageMultiplierPerSecondofCharge;

			Owner hitOwner = collision.gameObject.GetComponent<Owner>();
			if (hitOwner)
            {
				if (hitOwner.GetOwnerGuid() != ownerGuid)
                {
					dm.Damage(chargeDamage);
					HitInfo hitInfo = collision.gameObject.GetComponent<HitInfo>();
					hitInfo.SetLastHitBy(ownerGuid);

					Destroy(gameObject);
                }
            }
			else
            {
				dm.Damage(chargeDamage);
				Destroy(gameObject);
            }

        }
    }

	[Server]
	public void SetOwnerObject(GameObject ownerObject)
    {
		plasmaOwner = ownerObject;
    }

	[Server]
	public bool IncreaseCharge(float amount) {
		if (amount > 0) {
			charge += amount;
			if (charge > (maxChargeTime + initialCharge)) {
				charge = maxChargeTime + initialCharge;
				return false;
            }
            else
            {
				return true;
            }
		} 
		else
        {
			return false;
        }
	}

	[Server]
	public void SetRadius (float newRadius) {
		if (newRadius > 0) {
			radius = newRadius;

			SetChildRadius();
		}
	}
	
	public float GetRadius() {
		return radius;
	}

	[Server]
	public void Release() {
		charging = false;
	}

	[Server]
	void SetChildRadius()
    {
		for (int i = 0; i < plasmaSpheres.Length; i++)
		{
			plasmaSpheres[i].SetRadius(radius);
		}

		for (int i = 0; i < plasmaTrails.Length; i++)
		{
			plasmaTrails[i].SetRadius(radius);
		}
	}

	public static GameObject GetBulletPrefab() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Projectiles/Plasma");
	}

	public new static float GetRefireRate() {
		if (plasmaInfo == null) {
			CreatePlasmaInfo();
		}

		return plasmaInfo.refireRate;
	}

	public new static int GetBulletsPerShot() {
		if (plasmaInfo == null) {
			CreatePlasmaInfo();
		}

		return plasmaInfo.bulletsPerShot;
	}

	public static float GetArc()
    {
		if (plasmaInfo == null)
        {
			CreatePlasmaInfo();
        }

		return basePlasma.arc;
    }

	static void CreatePlasmaInfo() {
		basePlasma = GetBulletPrefab().GetComponent<Plasma>();
		plasmaInfo = new ShootingInfo();
		plasmaInfo.bulletsPerShot = basePlasma.bulletsPerShot;
		plasmaInfo.refireRate = basePlasma.refireRate;
	}	
}
