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
		if (followTarget && target != null) {
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

			angle = Angle.GetAngle(oldPos, currentPos);
			speed = Vector3.Distance(oldPos, currentPos);

			MoveStars();			
		}
	}

	/*void CreateCamera() {
		GameObject temp = new GameObject ();
		cameraObject = (GameObject)Instantiate (temp, new Vector3 (0, 0, 0), Quaternion.identity);
		//cameraObject.AddComponent<NetworkIdentity> ();
		//NetworkServer.Spawn (cameraObject);
		currentPos = cameraObject.transform.position;
		oldPos = currentPos;
		shipCam = cameraObject.AddComponent<Camera> ();
		Destroy (temp);
	}*/

	/*void CreateTextCanvas() {
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

	public void SetTarget(GameObject newTarget) {
		target = newTarget;
	}

	public void SetHeight(float newHeight) {
		if (newHeight >= 0) {
			height = newHeight;
		}
	}

	public void ChangeHeight(float change) {
		height += change;
		if (height < 0) {
			height = 0;
		}
	}

	public void Follow(bool toFollow) {
		followTarget = toFollow;
	}

	public void ToggleFollow() {
		followTarget = !followTarget;
	}

	/*public Camera getCamera() {
		return shipCam;
	}

	public void setScreenText(string overlayText) {
		text.text = overlayText;
	}*/

	public void SetSpeed(float newSpeed) {
		speed = newSpeed;
	}

	public void SetAngle(float newAngle) {
		angle = newAngle;
	}

	/*public void setCullLayer(int newCullMask) {
		shipCam.cullingMask = newCullMask;
	}*/

	void FixedUpdate() {

		
	}

	void MoveStars() {

		if (starField != null && target != null) {
			//print (angle + "  " + speed);
			starField.MoveStars(target.transform.position, angle, speed);
		}
	}
}
