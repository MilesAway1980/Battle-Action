using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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
	Ship owner;

	// Use this for initialization
	void Start () {

		//MeshFilter mf = GetComponent<MeshFilter>();
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

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
		if (players == null) {
			return;
		}

		for (int i = 0; i < players.Length; i++) {

			Ship playerShip = players[i].GetComponent<Ship>();

			float dist = Vector2.Distance(startPos, playerShip.transform.position);



			if (dist < range ) {
				bool damaged = false;

				float angleToTarget = Angle.getAngle(startPos, playerShip.transform.position);

				if (dist > 1) {
					float pointAngle = (360 - angle);
					if (angleToTarget < (pointAngle + 1) && angleToTarget > (pointAngle - 1)) {
						damaged = true;
					}

					if (damaged == false) {

						CircleCollider2D shipCollider = playerShip.GetComponent<CircleCollider2D>();

						if (Intersect.LineCircle(startPos, leftCorner, playerShip.transform.position, shipCollider.radius)) {
							damaged = true;
						}

						if (damaged == false) {
							if (Intersect.LineCircle(startPos, rightCorner, playerShip.transform.position, shipCollider.radius)) {
								damaged = true;
							}
						}
					}
				}

				if (damaged) {
					playerShip.damage (damage);
					playerShip.setLastHitBy(owner.getOwner ());
				}
			}
		}
	}

	public void init(Vector2 newStartPos, float newAngle, Ship newOwner) {
		startPos = newStartPos;
		angle = newAngle;
		owner = newOwner;
	}

	public static GameObject getBlaster() {
		return (GameObject)Resources.Load ("Prefabs/Weapons/Blaster");
	}
}
