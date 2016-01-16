using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Blaster : NetworkBehaviour {

	[SyncVar] Vector2 startPos;
	[SyncVar] float angle;
	[SyncVar] Vector2 leftCorner;
	[SyncVar] Vector2 rightCorner;

	[Range (0, 15)]
	public float spread;
	public float range;
	public float damage;

	GameObject blasterObject;

	public Material mat;

	GameObject owner;

	// Use this for initialization
	void Start () {
		
		MeshRenderer mr = GetComponent<MeshRenderer>();
		mr.material = mat;
	}

	void FixedUpdate() {
		setShape ();
		checkDamage ();
	}

	void setShape() {

		Mesh beamMesh = GetComponent<MeshFilter>().mesh;
		beamMesh.Clear();

		float leftAngle = (angle - spread) / Mathf.Rad2Deg;
		float rightAngle = (angle + spread) / Mathf.Rad2Deg;

		leftCorner = new Vector2 (
			startPos.x - Mathf.Sin (leftAngle) * range,
			startPos.y + Mathf.Cos (leftAngle) * range
		);

		rightCorner = new Vector2 (
			startPos.x - Mathf.Sin (rightAngle) * range,
			startPos.y + Mathf.Cos (rightAngle) * range
		);
		
		beamMesh.vertices = new Vector3[] { 
			//new Vector3(0, 0, 0), 
			//new Vector2(0, 10.0f, 0), 
			//new Vector2(10.0f, 10.0f, 0) 
			startPos,
			rightCorner,
			leftCorner
		};
		
		beamMesh.uv = new Vector2[] { 
			startPos,
			rightCorner,
			leftCorner
		};

		beamMesh.triangles = new int[] { 0, 1, 2 };
	}

	void checkDamage() {

		if (!isServer) {
			return;
		}

		//Get a list of all damageable objects
		List<GameObject> allObjects = Damageable.damageableList.getObjectList();
		if (allObjects.Count == 0) {
			return;
		}

		for (int i = 0; i < allObjects.Count; i++) {
			float dist = Vector2.Distance(startPos, allObjects[i].transform.position);
			if (dist < range && dist > 1) {
				Damageable target = allObjects [i].GetComponent<Damageable> ();
				GameObject targetOwner = target.gameObject;
				Vector3 targetPos = targetOwner.transform.position;

				bool damaged = false;

				float angleToTarget = Angle.getAngle (startPos, targetPos);
				float pointAngle = (360 - angle);



				if (angleToTarget < (pointAngle + spread) && angleToTarget > (pointAngle - spread)) {
					damaged = true;
				}

				if (damaged == false) {
					CircleCollider2D collider = targetOwner.GetComponent<CircleCollider2D> ();
					if (collider) {
						if (Intersect.LineCircle (startPos, leftCorner, targetPos, collider.radius, false)) {
							damaged = true;
						}

						if (damaged == false) {
							if (Intersect.LineCircle (startPos, rightCorner, targetPos, collider.radius, false)) {
								damaged = true;
							}
						}
					}
				}

				if (damaged) {
					target.damage (damage);
					HitInfo info = targetOwner.GetComponent<HitInfo> ();
					if (info) {						
						info.setLastHitBy (owner);
					}
				}
			}
		}
	}

	public void init(Vector2 newStartPos, float newAngle, GameObject newOwner) {
		startPos = newStartPos;
		angle = newAngle;
		owner = newOwner;
	}

	public static GameObject getBlaster() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Blaster");
	}
}
