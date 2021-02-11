using UnityEngine;
using Mirror;
using System.Collections;
using System;

public class Mine : NetworkBehaviour
{
	public float damage;

	public float ownerScale;
	public float nonOwnerScale;

	[SyncVar] Guid ownerGuid;
	
	// Use this for initialization
	void Start()
	{		
		if (!isServer)
        {
			return;
        }

		transform.localScale = new Vector3(ownerScale, ownerScale, ownerScale);
		RpcSizeForLocalPlayer(ownerGuid);
	}

	[ClientRpc]
	void RpcSizeForLocalPlayer(Guid ownerGuid)
    {
		Player thisPlayer = Player.GetLocalPlayer();
		if (thisPlayer == null)
		{
			return;
		}

		Vector3 size;
		if (ownerGuid == thisPlayer.GetPlayerGuid())
		{
			size = new Vector3(ownerScale, ownerScale, ownerScale);
		}
		else
		{
			size = new Vector3(nonOwnerScale, nonOwnerScale, nonOwnerScale);
		}

		transform.localScale = size;
	}

	void Update()
	{
		
	}

	[Server]
	public void Init(Guid ownerGuid)
	{
		this.ownerGuid = ownerGuid;
	}

	public static GameObject GetMine()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Mine");
	}	

	public GameObject GetExplosion()
	{
		return (GameObject)Resources.Load("Prefabs/Exploder");
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
			Owner hitOwner = collision.gameObject.GetComponent<Owner>();
			if (hitOwner)
            {
				if (hitOwner.GetOwnerGuid() != ownerGuid)
                {
					dm.Damage(damage);
					HitInfo hitInfo = collision.gameObject.GetComponent<HitInfo>();
					hitInfo.SetLastHitBy(ownerGuid);

					GameObject explosion = Instantiate(
						GetExplosion(),
						transform.position,
						Quaternion.identity
					);

					explosion.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

					NetworkServer.Spawn(explosion);
					Destroy(gameObject);
                }
            }
			else
            {
				dm.Damage(damage);
            }				
        }
    }
}
