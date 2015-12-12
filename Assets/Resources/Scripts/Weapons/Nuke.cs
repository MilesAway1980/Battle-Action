using UnityEngine;
using System.Collections;

public class Nuke : MonoBehaviour {

	// Use this for initialization
	void Start () {
<<<<<<< HEAD
		currentTime = 0;
		exploded = false;
		nukeSize = new Vector3 (0.3f, 0.3f, 0.3f);
		transform.localScale = nukeSize;
	}

	void FixedUpdate () {

		print (this.transform.parent);
		print (this.transform.position);

		if (exploded == false) {
			currentTime += Time.deltaTime;

			if (owner != null) {
				angle = owner.getAngle () / Mathf.Rad2Deg;
				pos = new Vector2 (
					owner.transform.position.x - Mathf.Sin (angle) * -2,
					owner.transform.position.y + Mathf.Cos (angle) * -2
				);
			}

			if (currentTime >= detonatorTime) {
				exploded = true;

				nukeSize = new Vector3(0, 0, 0);
				fireballSize = new Vector3(0, 0, 0);
				transform.localScale = nukeSize;

				thisFireball = (GameObject)Instantiate (fireball);
				thisFireball.transform.position = pos;
				thisFireball.transform.localScale = fireballSize;
			}

		} else {
			currentRadius += expansionSpeed;

			float cameraHeight = Mathf.Abs(Camera.main.transform.position.z);
			float explosionHeight = (currentRadius / maxRadius) * cameraHeight;

			fireballSize = new Vector3(currentRadius, currentRadius, explosionHeight);
			if (thisFireball != null) {
				thisFireball.transform.localScale = fireballSize;
			}
			checkDamage();

			if (currentRadius >= maxRadius) {
				Destroy (this.gameObject);
				Destroy (thisFireball);
			}
		}

		transform.position = pos;
	}

	[Server]
	void checkDamage() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player Ship");
		if (players == null) {
			return;
		}

		for (int i = 0; i < players.Length; i++) {

			Ship whichShip = players[i].GetComponent<Ship>();
			if (whichShip == owner) {
				continue;
			}

			if (Vector2.Distance(
				pos,
				whichShip.transform.position
			) <= (currentRadius / 2) ) {
				whichShip.damage(damage);
			}
		}
	}

	public void init(Ship newOwner) {
		owner = newOwner;
	}

	public static GameObject getBomb() {
		return (GameObject)Resources.Load ("Prefabs/3D Weapons/Nuke");
=======
	
>>>>>>> parent of da0b892... Added Nuke weapon. Began work on Crush.
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
