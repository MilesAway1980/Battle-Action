using UnityEngine;
using System.Collections;

public class StarField : MonoBehaviour
{

	public int numStars = 200;
	public static int starLayer = 6;
	public int starDepth = 2;
	public bool updateMovement = true;
	Star[] stars;

	// Use this for initialization
	void Start()
	{
		stars = new Star[numStars];

		//Create a game object to hold all of the stars
		GameObject starContainer = new GameObject
		{
			name = "Stars"
		};

		starContainer.transform.parent = transform;

		GameObject star = (GameObject)Resources.Load("Prefabs/Star", typeof(GameObject));

		float minX = -20;
		float maxX = 20;
		float minY = -20;
		float maxY = 20;

		Star.SetStarDepth(starDepth);

		for (int i = 0; i < stars.Length; i++)
		{

			stars[i] = new Star();

			Vector3 starPos = new Vector3(
				Random.Range(minX, maxX),
				Random.Range(minY, maxY),
				starDepth
			);

			GameObject newStar = Instantiate(
				star,
				starPos,
				Quaternion.identity,
				starContainer.transform
			);

			stars[i].Init(newStar);
			stars[i].SetStarLayer(starLayer);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetStarLayer(int newLayer)
	{
		starLayer = newLayer;
		if (stars != null)
		{
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].SetStarLayer(starLayer);
			}
		}
	}

	public void MoveStars(Vector2 camPos, float angle, float speed)
	{

		if (!updateMovement)
		{
			return;
		}

		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].Move(camPos, angle, speed);
		}
	}
}
