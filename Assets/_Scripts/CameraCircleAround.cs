using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCircleAround : MonoBehaviour
{
    Camera circleCamera;
    public GameObject targetObject;
    public Vector3 targetPos;
    public float distance;
    public float rise;
    public float speed;

    float angleRad;
    Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        angleRad = Random.Range(0, Angle.DoublePi);
        circleCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!circleCamera)
        {
            return;
        }

        angleRad += speed;
        //float angleRad = angle * Mathf.Deg2Rad;

        pos = new Vector3(
            Mathf.Cos(angleRad) * distance,
            rise,
            Mathf.Sin(angleRad) * distance
        );

        circleCamera.transform.position = pos;
        if (targetObject)
        {
            targetPos = targetObject.transform.position;
        }

        circleCamera.transform.LookAt(targetPos);
    }
}
