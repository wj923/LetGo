using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveManager : MonoBehaviour {

    Camera mainCam;
    public Camera subCam;
    GameObject target = null;
    bool moveCam = false;
    public GameObject center;

    private Transform TargetLookAt;
    public float Distance = 100.0f;
    public float DistanceMin = 20.0f;
    public float DistanceMax = 500.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    private float startingDistance = 0.0f;
    private float desiredDistance = 0.0f;
    public float X_MouseSensitivity = 200.0f;
    public float Y_MouseSensitivity = 200.0f;
    public float ZoomSensitivity = 300.0f;
    public float Y_MinLimit = -40.0f;
    public float Y_MaxLimit = 80.0f;
    public float DistanceSmooth = 0.05f;
    private float velocityDistance = 0.0f;
    private Vector3 desiredPosition = Vector3.zero;
    public float X_Smooth = 0.05f;
    public float Y_Smooth = 0.1f;
    private float velX = 0.0f;
    private float velY = 0.0f;
    private float velZ = 0.0f;
    private Vector3 position = Vector3.zero;
    private float initTouchDist;
    private Vector2 initTouchPos;

    private void Start()
    {
        subCam.enabled = false;
        mainCam = Camera.main;
        mainCam.transform.LookAt(center.transform);

        TargetLookAt = center.transform;
        Distance = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
        startingDistance = Distance;
        Reset();
    }

    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            target = GetTarget();
            if (target != null) // Bricks Movement
            {
                subCam.enabled = true;
                subCam.transform.position = mainCam.transform.position + target.transform.position;
                subCam.transform.LookAt(target.transform);
                target.GetComponent<Bricks>().movement = true;
                target.GetComponent<Bricks>().subCam = subCam; // Send message to < Script "Bricks" > 
                target.layer = LayerMask.NameToLayer("Ignore Raycast"); // Don't perceive target Bricks
                // To-do : What color on selecting..?
            }
            else // Camera Rotation
            {
                moveCam = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if(target != null) // Bricks Movement End
            {
                subCam.enabled = false;
                target.GetComponent<Bricks>().movement = false; // Send message to < Script "Bricks" > 
                target.layer = LayerMask.NameToLayer("Default"); // Rollback perceiving
                target.GetComponent<Bricks>().saveChange(); // save Lotation and Rotation data
                target = null;
            }
            else // Camera Rotation End
            {
                moveCam = false;
            }
        }
	}

    private void LateUpdate() // Handling About Camera Rotation and Zoom
    {
        if (moveCam && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            HandlePlayerInput();
            CalculateDesiredPosition();
            UpdatePosition();
        }
    }

    GameObject GetTarget()
    {
        RaycastHit hit;
        GameObject target = null;
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity) && hit.transform.tag == "Brick")
        {
            target = hit.transform.gameObject;
        }
        return target;
    }

    void HandlePlayerInput()
    {
        if(Input.touchCount == 1) // Camera Rotation
        {
            if(Input.GetTouch(0).position != initTouchPos)
            {
                mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity * Time.deltaTime;
                mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity * Time.deltaTime;
                mouseY = ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);
            }
            initTouchPos = Input.GetTouch(0).position;
        }

        else if (Input.touchCount == 2) // Camera Zoom
        {
            if(Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) != initTouchDist)
            {
                desiredDistance = Mathf.Clamp(Distance - (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - initTouchDist)* ZoomSensitivity * Time.deltaTime, DistanceMin, DistanceMax);
            }

            initTouchDist = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
        }
    }
    void CalculateDesiredPosition()
    {
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velocityDistance, DistanceSmooth);
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }
    Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return TargetLookAt.position + (rotation * direction);
    }
    void UpdatePosition()
    {
        float posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        float posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        float posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);

        this.transform.position = position;

        this.transform.LookAt(TargetLookAt);
    }
    void Reset()
    {
        mouseX = 0;
        mouseY = 30;
        Distance = startingDistance;
        desiredDistance = Distance;
    }

    float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360 || angle > 360)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
