using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ShipCamera : MonoBehaviour {

	//GameObject cameraObject;
	//Camera shipCam;
	GameObject target;
	bool followTarget;
	float height;
	string overlay;

	float speed;
	float angle;

	//Canvas canvas;
	//Text text;

	Vector3 currentPos;
	Vector3 oldPos;

	StarField starField;

	// Use this for initialization
	void Awake () {

		followTarget = true;
		//createCamera ();
		//createTextCanvas ();

		starField = gameObject.AddComponent<StarField>();
		currentPos = Camera.main.transform.position;
		oldPos = currentPos;
	}
	
	// Update is called once per frame
	void Update () {
		if (followTarget) {
			if (target != null)
			{
				if (target.transform.position != currentPos)
				{
					oldPos = currentPos;
					Camera.main.transform.position = new Vector3 (
							target.transform.position.x,
							target.transform.position.y,
							-height
						);
					currentPos = Camera.main.transform.position;
				}

				moveStars();
			}
		}
	}

	/*void createCamera() {
		GameObject temp = new GameObject ();
		cameraObject = (GameObject)Instantiate (temp, new Vector3 (0, 0, 0), Quaternion.identity);
		//cameraObject.AddComponent<NetworkIdentity> ();
		//NetworkServer.Spawn (cameraObject);
		currentPos = cameraObject.transform.position;
		oldPos = currentPos;
		shipCam = cameraObject.AddComponent<Camera> ();
		Destroy (temp);
	}*/

	/*void createTextCanvas() {
		GameObject newCanvas = new GameObject ();
		
		GameObject canvasObject = (GameObject)Instantiate (newCanvas, new Vector3 (0, 0, 0), Quaternion.identity);
		canvas = canvasObject.AddComponent<Canvas> ();
		text = canvas.gameObject.AddComponent<Text> ();
		
		canvas.renderMode = RenderMode.ScreenSpaceCamera;
		canvas.planeDistance = 1;
		canvas.worldCamera = shipCam;
		
		canvas.transform.SetParent (shipCam.transform);
		canvas.name = "Player Canvas";
		
		text.font = Resources.GetBuiltinResource<Font> ("Arial.ttf");
		text.fontStyle = FontStyle.Normal;
		text.text = "Battle Action!!";
		
		Destroy (newCanvas);
	}*/

	public void setTarget(GameObject newTarget) {
		target = newTarget;
	}

	public void setHeight(float newHeight) {
		if (newHeight >= 0) {
			height = newHeight;
		}
	}

	public void changeHeight(float change) {
		height += change;
		if (height < 0) {
			height = 0;
		}
	}

	public void follow(bool toFollow) {
		followTarget = toFollow;
	}

	public void toggleFollow() {
		followTarget = !followTarget;
	}

	/*public Camera getCamera() {
		return shipCam;
	}

	public void setScreenText(string overlayText) {
		text.text = overlayText;
	}*/

	public void setSpeed(float newSpeed) {
		speed = newSpeed;
	}

	public void setAngle(float newAngle) {
		angle = newAngle;
	}

	/*public void setCullLayer(int newCullMask) {
		shipCam.cullingMask = newCullMask;
	}*/

	void FixedUpdate() {
		angle = Angle.getAngle(oldPos, currentPos);
		speed = Vector3.Distance (oldPos, currentPos);
		moveStars ();
	}

	void moveStars() {

		if (starField != null) {
			//print (angle + "  " + speed);
			starField.moveStars(transform.position, angle, speed);
		}
	}
}
