using UnityEngine;
using Mirror;
using System;

public class MineField : NetworkBehaviour
{

	[Range(1, 100)]
	public int minesPerDrop;
	public float radius;
	public float refireRate;
	static float mineRefireRate = -1;

	Mine[] mines;

	Guid ownerGuid;

	// Use this for initialization
	void Start()
	{
		mines = new Mine[minesPerDrop];
		PlaceMines();
		
	}

	[Server]
	void PlaceMines()
	{
		print($"placing {minesPerDrop} mines");

		for (int i = 0; i < minesPerDrop; i++)
		{
			float distance = UnityEngine.Random.Range(0.0f, radius);
			float angleRad = UnityEngine.Random.Range(0.0f, Angle.DoublePi);

			Vector2 pos = transform.position;

			Vector2 minePos = new Vector2(
				pos.x + Mathf.Cos(angleRad) * distance,
				pos.y + Mathf.Sin(angleRad) * distance
			);

			GameObject mineObject = Instantiate(
				Mine.GetMine(),
				minePos,
				Quaternion.identity
			);

			Mine mine = mineObject.GetComponent<Mine>();
			mines[i] = mine;
			mine.Init(ownerGuid);

			mine.transform.parent = transform;

			NetworkServer.Spawn(mineObject);
		}
	}

	public void FixedUpdate()
	{
		if (!isServer)
		{
			return;
		}

		bool hasMines = false;
		for (int i = 0; i < mines.Length; i++)
		{
			if (mines[i] != null)
			{
				hasMines = true;
				break;
			}
		}

		if (!hasMines)
		{
			Destroy(this);
		}
		else
		{
			print("Has mines");
		}
	}

	public void Init(Guid ownerGuid)
	{
		this.ownerGuid = ownerGuid;
	}

	public static GameObject GetMineField()
	{
		return (GameObject)Resources.Load("Prefabs/Weapons/MineField");
	}

	public static float GetRefireRate()
	{
		if (mineRefireRate < 0)
		{
			mineRefireRate = GetMineField().GetComponent<MineField>().refireRate;
		}

		return mineRefireRate;
	}
}
