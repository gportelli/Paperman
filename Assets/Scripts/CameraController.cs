using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public float cameraDistance = 2;
    public float rotationOffsetY = 10f;
    public float rotationStartX = 20f;
    public float maxXRotation = 60f;
    public float minXRotation = -60f;
    public float minXRotationAuto = -10;
    public float maxXRotationAuto = 30;
    public float rotationSpeed = 90;
    public float rotationSmoothTime = 1f;
    public float rotationTimeout = 1;

    public float accelerationSensitivity = 0.1f;
    public float transitionTime = 1f;
    public float maxOffset = 10;

    private Vector3 accelerationOffset;
    private Vector3 currAccelerationOffset;

    private Vector3 rotation, currentRotation;
    private float rotationTime;

    private GameObject player;
    private PlayerController playerController;

    private Vector3 currentVelocity, currentRotVelocity;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>() as PlayerController;

        currAccelerationOffset = accelerationOffset = Vector3.zero;

        currentRotation = rotation = new Vector3(rotationStartX, rotationOffsetY, 0);

		gameObject.transform.position = player.transform.position  + GetOffset();
	}

    Vector3 GetOffset()
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = currentRotation + new Vector3(0, player.transform.eulerAngles.y, 0);

        return q * new Vector3(0, 0, -cameraDistance);
    }

	void LateUpdate () {
        RotateCamera();

        accelerationOffset = -playerController.getAcceleration() * accelerationSensitivity;

        if (accelerationOffset.magnitude > maxOffset)
            accelerationOffset = accelerationOffset.normalized * maxOffset;

        currAccelerationOffset = Helper.SmoothDampVector3(currAccelerationOffset, accelerationOffset, ref currentVelocity, transitionTime);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);

        gameObject.transform.position = player.transform.position + GetOffset() + currAccelerationOffset;
        gameObject.transform.LookAt(player.transform.position);
        
        /*
        Vector3 offset = new Vector3(0, cameraHeight, cameraDistance);

        Vector3 vel = playerController.GetVelocity();

        if (vel.magnitude > 0)
        {
            Vector3 velYplane = new Vector3(vel.x, 0, vel.z);
            //if (vel.y / velYplane.magnitude < 0.2)
            //{
                q = Quaternion.LookRotation(-velYplane);
            //}
        }
            
        gameObject.transform.position = player.transform.position + q * offset + currAccelerationOffset;

        gameObject.transform.eulerAngles = 
            new Vector3(
                gameObject.transform.eulerAngles.x,
                Quaternion.LookRotation(player.transform.position - gameObject.transform.position).eulerAngles.y,
                gameObject.transform.eulerAngles.z);
        */
	}

    private void RotateCamera()
    {
        float rh = Input.GetAxis("Analog2Horiz"); 
        float rv = Input.GetAxis("Analog2Vert");
        
        rotation.y += rh * rotationSpeed * Time.deltaTime;
        if (rotation.y > 360) rotation.y -= 360;
        else if(rotation.y < 0) rotation.y += 360;

        rotation.x += rv * rotationSpeed * Time.deltaTime;
        if (rotation.x > maxXRotation) rotation.x = maxXRotation;
        else if (rotation.x < minXRotation) rotation.x = minXRotation;

        if (rh != 0 || rv != 0)
            rotationTime = rotationTimeout;

        if (rotationTime > 0)
        {
            rotationTime -= Time.deltaTime;

            if (rotationTime <= 0)
            {
                rotationTime = 0;                
                
                if (rotation.y > 180) rotation.y -= 360;
                rotation.y = rotation.y > 0 ? rotationOffsetY : -rotationOffsetY;

                if (rotation.x > maxXRotationAuto) rotation.x = maxXRotationAuto;
                else if (rotation.x < minXRotationAuto) rotation.x = minXRotationAuto;
            }
        }

        if (currentRotation.y - rotation.y > 180) rotation.y += 360;
        else if (currentRotation.y - rotation.y < -180) currentRotation.y += 360;

        currentRotation = Helper.SmoothDampVector3(currentRotation, rotation, ref currentRotVelocity, rotationSmoothTime);
    }
}
