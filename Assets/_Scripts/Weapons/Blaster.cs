using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System;

public class Blaster : NetworkBehaviour
{

	[SyncVar] Vector2 startPos;
	[SyncVar] float angle;
	//[SyncVar] Vector2 leftCorner;
	//[SyncVar] Vector2 rightCorner;

	[Range(0, 15)]
	public float spread;
	public float range;
	public float damage;

	public Material mat;
	Guid ownerGuid;

	GameObject owner;
	PolygonCollider2D beamCollider;

	// Use this for initialization
	void Start()
	{
		if (!owner)
        {
			return;
        }

		float leftAngle = -spread * Mathf.Deg2Rad;
		float rightAngle = spread * Mathf.Deg2Rad;

		Vector2 leftCorner = new Vector2(
			-Mathf.Sin(leftAngle) * range,
			Mathf.Cos(leftAngle) * range
		);

		Vector2 rightCorner = new Vector2(
			-Mathf.Sin(rightAngle) * range,
			Mathf.Cos(rightAngle) * range
		);

		beamCollider = GetComponent<PolygonCollider2D>();		

		Vector2[] corners = new Vector2[3];
		corners[0] = Vector2.zero;
		corners[1] = leftCorner;
		corners[2] = rightCorner;

		beamCollider.points = corners;

		RpcDrawBlasterMesh(leftCorner, rightCorner);
	}

	void FixedUpdate()
	{
		if (!isServer || !owner)
		{
			return;
		}

		transform.position = owner.transform.position;
		transform.rotation = owner.transform.rotation;
	}		

	[ClientRpc]
	void RpcDrawBlasterMesh(Vector2 leftCorner, Vector2 rightCorner)
    {
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.material = mat;		

		Mesh beamMesh = GetComponent<MeshFilter>().mesh;
		beamMesh.Clear();

		beamMesh.vertices = new Vector3[] {
			Vector3.zero,
			rightCorner,
			leftCorner
		};

		beamMesh.uv = new Vector2[] {
			Vector3.zero,
			rightCorner,
			leftCorner
		};

		beamMesh.triangles = new int[] { 0, 1, 2 };
	}

	private void OnTriggerStay2D(Collider2D collision)
    {
		Damageable dm = collision.gameObject.GetComponent<Damageable>();
		if (dm)
        {
			Owner damageOwner = collision.gameObject.GetComponent<Owner>();			

			if (damageOwner.GetOwnerGuid() == ownerGuid)
            {
				return;
            }

			HitInfo hitInfo = collision.gameObject.GetComponent<HitInfo>();
			if (hitInfo)
            {
				hitInfo.SetLastHitBy(ownerGuid);
            }

			dm.Damage(damage);
        }
    }

    public void Init(Guid ownerGuid)
	{
		owner = Owner.FindOwnerByGuid(ownerGuid).gameObject;
		startPos = owner.transform.position;
		angle = owner.transform.eulerAngles.z;		

		this.ownerGuid = ownerGuid;
	}

	public static GameObject getBlaster()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/Blaster");
	}
}
