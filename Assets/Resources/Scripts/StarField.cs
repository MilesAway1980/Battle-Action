using UnityEngine;
using System.Collections;

public class StarField : MonoBehaviour {

	public int numStars = 200;
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

			GameObject newStar = Instantiate(star, new Vector2(0, 0), Quaternion.identity);

			stars[i].Init (newStar, starPos, starContainer.transform);
			stars[i].SetStarLayer(starLayer);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetStarLayer(int newLayer) {
		starLayer = newLayer;
		if (stars != null) {
			for (int i = 0; i < stars.Length; i++) {
				stars[i].SetStarLayer(starLayer);
			}
		}
	}

	public void MoveStars(Vector2 camPos, float angle, float speed) {
		//print($"{camPos.x}, {camPos.y}   -  {angle}  - {speed}");
		for (int i = 0; i < stars.Length; i++) {
			stars[i].Move(camPos, angle, speed);
		}
	}
}
