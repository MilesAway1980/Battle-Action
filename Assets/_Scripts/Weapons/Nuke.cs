using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class Nuke : NetworkBehaviour {

	static float nukeRefireRate = -1;

	public float refireRate;
	public float damage;
	public float detonatorTime;
	public float maxRadius;
	public float expansionSpeed;

	public GameObject fireball;
	public GameObject bomb;

	[SyncVar] float currentTime;
	[SyncVar] float currentRadius;
	[SyncVar] Vector2 pos;
	[SyncVar] float bombSize;

	TrailRenderer tether;
	Vector3[] tetherPositions = new Vector3[2000];

	Guid ownerGuid;
	GameObject owner;

	[SyncVar] bool exploded;

	// Use this for initialization
	void Start () {
		if (!isServer)
        {
			return;
        }

		currentTime = 0;
		exploded = false;
		
		fireball.transform.localScale = Vector3.zero;		

		bombSize = bomb.transform.localScale.x;

		
		//tether.Clear();
	}

	void FixedUpdate () {

		//print(ownerGuid);

		if (!isServer || ownerGuid == Guid.Empty)
        {
			return;
        }

		if (exploded == false) {
			currentTime += Time.deltaTime;
			
			float angle = owner.transform.eulerAngles.z * Mathf.Deg2Rad;
			/*pos = new Vector2 (
				owner.transform.position.x - Mathf.Sin (angle) * -2,
				owner.transform.position.y + Mathf.Cos (angle) * -2
			);*/
			
			int numPos = tether.GetPositions(tetherPositions);
			print(numPos);

			pos = tether.GetPosition(0);

			if (currentTime >= detonatorTime) {
				exploded = true;
				fireball.transform.position = pos;
			} 

		} else {
			currentRadius += expansionSpeed;

			bombSize -= 0.1f;
			if (bombSize < 0)
            {
				bombSize = 0;
            }

			bomb.transform.localScale = new Vector3(bombSize, bombSize, bombSize);

			float cameraHeight = Mathf.Abs(Camera.main.transform.position.z);
			float explosionHeight = (currentRadius / maxRadius) * cameraHeight;

			fireball.transform.localScale = new Vector3(currentRadius, currentRadius, explosionHeight);

			if (currentRadius >= maxRadius) {				
				Destroy (gameObject);
				Destroy (tether.gameObject);
			}
		}

		transform.position = pos;
	}

    private void OnTriggerStay2D(Collider2D collision)
    {
		Damageable dm = collision.gameObject.GetComponent<Damageable>();
		if (dm)
        {
			Owner hitOwner = collision.gameObject.GetComponent<Owner>();
			if (hitOwner && hitOwner.GetOwnerGuid() != ownerGuid)
            {
				dm.Damage(damage);
				HitInfo hitInfo = collision.gameObject.GetComponent<HitInfo>();
				if (hitInfo)
                {
					hitInfo.SetLastHitBy(ownerGuid);
                }
            }
        }
    }

    public void Init(Guid ownerGuid, GameObject owner) {
		this.owner = owner;
		this.ownerGuid = ownerGuid;

		tether = GetComponentInChildren<TrailRenderer>();
		tether.gameObject.transform.parent = owner.transform;
		tether.gameObject.transform.position = owner.transform.position;
		tether.Clear();
	}

	public static GameObject GetBomb() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Nuke");
	}
	
	public static float GetRefireRate() {
		if (nukeRefireRate == -1) {
			Nuke nuke = GetBomb().GetComponent<Nuke>();
			nukeRefireRate = nuke.refireRate;
		}

		return nukeRefireRate;
	}
}
