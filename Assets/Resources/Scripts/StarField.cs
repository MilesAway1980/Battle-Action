using UnityEngine;
using System.Collections;

class Star {

	GameObject starObject;
	SpriteRenderer sr;
	Vector2 pos;

	float size;
	float speed;

	int maxXDistance = 11;
	int maxYDistance = 6;

	int starLayer = 0;

	public void init(GameObject newSpriteObject, Vector2 newPos, Transform parent) {
		starObject = newSpriteObject;
		starObject.transform.position = newPos;
		starObject.transform.parent = parent;
		sr = starObject.GetComponent<SpriteRenderer> ();
		newSpriteObject.layer = starLayer;

		resetStar ();

		sr.transform.localScale = new Vector2 (size, size);

		pos = newPos;
	}

	public void move(Vector2 camPos, float angle, float moveSpeed) {

		float radAngle = angle / Mathf.Rad2Deg;

		pos = new Vector2 (
			pos.x - Mathf.Sin(radAngle) * (moveSpeed * speed),
			pos.y - Mathf.Cos(radAngle) * (moveSpeed * speed)
		);

		Vector2 screenCoord = new Vector2 (camPos.x + pos.x, camPos.y + pos.y);

		if (screenCoord.x < (camPos.x - maxXDistance)) {
			pos = new Vector2(pos.x + maxXDistance * 2, pos.y);
			resetStar();
		} else if (screenCoord.x > (camPos.x + maxXDistance)) {
			pos = new Vector2(pos.x - maxXDistance * 2, pos.y);
			resetStar();
		}

		if (screenCoord.y < (camPos.y - maxYDistance)) {
			pos = new Vector2 (pos.x, pos.y + maxYDistance * 2);
			resetStar();
		} else if (screenCoord.y > (camPos.y + maxYDistance)) {
			pos = new Vector2 (pos.x, pos.y - maxYDistance * 2);
			resetStar();
		}

		starObject.transform.position = screenCoord;


	}

	public void setStarLayer(int newLayer) {
		if (newLayer >= 0 && newLayer < 32) {
			starLayer = newLayer;
			starObject.layer = newLayer;
		}
	}

	void resetStar() {
		size = (float)Random.Range (0.0f, 0.25f);
		speed = (float)Random.Range (0.0f, 0.25f);
	}


}

public class StarField : MonoBehaviour {

	int numStars = 200;
	int starLayer = 0;
	Star[] stars;

	// Use this for initialization
	void Start () {
		stars = new Star[numStars];

		//Create a game object to hold all of the stars
		GameObject starContainer = new GameObject ();
		starContainer.name = "Stars";
		starContainer.transform.parent = transform;

		GameObject star = (GameObject)Resources.Load ("Prefabs/Star", typeof(GameObject));

		float minX = -10;
		float maxX = 10;
		float minY = -10;
		float maxY = 10;

		for (int i = 0; i < stars.Length; i++) {
			stars[i] = new Star();
			Vector2 starPos = new Vector2(
					Random.Range (minX, maxX),
					Random.Range (minY, maxY)
				);

			GameObject newStar = (GameObject)Instantiate (star, new Vector2(0, 0), Quaternion.identity);

			stars[i].init (newStar, starPos, starContainer.transform);
			stars[i].setStarLayer(starLayer);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setStarLayer(int newLayer) {
		starLayer = newLayer;
		if (stars != null) {
			for (int i = 0; i < stars.Length; i++) {
				stars [i].setStarLayer (starLayer);
			}
		}
	}

	public void moveStars(Vector2 camPos, float angle, float speed) {
		for (int i = 0; i < stars.Length; i++) {
			stars[i].move(camPos, angle, speed);
		}
	}
}
