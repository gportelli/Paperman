using UnityEngine;
using System.Collections;
using Rewired;

public class CameraController : MonoBehaviour {
    public enum CameraModes
    {
        Follow,
        Fixed,
        Disabled
    };

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
    public float transitionFixedTime = 2f;
    public float maxOffset = 10;

    private Vector3 accelerationOffset;
    private Vector3 currAccelerationOffset;

    private Vector3 rotation;
    private Quaternion currentRotation;
    private float rotationTime;

    private GameObject player;
    private PlayerController playerController;

    private Vector3 currentVelocity, currentDampVelocity;

    private CameraModes mode;
    private Vector3 fixedPosition;

    //private Vector3 fixedPosition, fixedOffset;
    //float fixedOffsetDuration, fixedOffsetProgress;
    //bool followTransition = false;

    private int cameraMode;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>() as PlayerController;
    }

	// Use this for initialization
	void Start () {        
       
	}

    public void Init()
    {
        currAccelerationOffset = accelerationOffset = Vector3.zero;

        rotation = new Vector3(rotationStartX, rotationOffsetY, 0);
        currentRotation = new Quaternion();
        currentRotation.eulerAngles = rotation;

        ResetCameraPosition();
    }

    private Vector3 GetTargetLookAt()
    {
        float targetLookAtDistance = 1f;

        Vector3 cameraWorldDirection = transform.position - player.transform.position;
        Vector3 targetLookAtWorldVector = player.transform.TransformDirection(Vector3.forward);
        
        cameraWorldDirection.y = 0;
        targetLookAtWorldVector.y = 0;

        float k = -Vector3.Dot(cameraWorldDirection.normalized, targetLookAtWorldVector.normalized);
        targetLookAtDistance = targetLookAtDistance * Mathf.Max(0, k) * targetLookAtDistance;

        Vector3 tla = player.transform.TransformPoint(Vector3.forward * targetLookAtDistance);

        Debug.DrawLine(player.transform.position, tla);

        return tla;
    }

    public void SetFollow(bool smoothTransition = true)
    {
        mode = CameraModes.Follow;

        //followTransition = smoothTransition;

        currentVelocity = Vector3.zero;
    }

    public void SetFixed(Vector3 fixedPosition, Vector3 startPosition)
    {
        if(startPosition != Vector3.zero)
            transform.position = startPosition;

        mode = CameraModes.Fixed;
        this.fixedPosition = fixedPosition;
        currentVelocity = Vector3.zero;
    }

    public void SetDisabled()
    {
        mode = CameraModes.Disabled;
    }

    Vector3 GetOffset()
    {
        return currentRotation * new Vector3(0, 0, -cameraDistance);
    }

    void ResetCameraPosition()
    {
        RotateCamera();
        transform.position = player.transform.position + GetOffset();
    }

	void FixedUpdate () {
        if (PlayerInput.Instance.Input.GetButtonDown("CameraToggle"))
            cameraMode = 1 - cameraMode;

        switch(mode)
        {
            case CameraModes.Follow:
                RotateCamera();

                accelerationOffset = -playerController.getAcceleration() * accelerationSensitivity;

                if (accelerationOffset.magnitude > maxOffset)
                    accelerationOffset = accelerationOffset.normalized * maxOffset;

                currAccelerationOffset = Vector3.SmoothDamp(currAccelerationOffset, accelerationOffset, ref currentVelocity, transitionTime);
                /*
                if (followTransition)
                {
                    followTransition = false;
                    fixedOffset = transform.position - (player.transform.position + GetOffset() + currAccelerationOffset);
                    fixedOffsetDuration = 1f;
                    fixedOffsetProgress = 0;
                }

                if(fixedOffsetProgress != 1) {
                    fixedOffsetProgress += Time.deltaTime / fixedOffsetDuration;
                    if (fixedOffsetProgress > 1) fixedOffsetProgress = 1;

                    fixedOffset = Vector3.Lerp(fixedOffset, Vector3.zero, Mathf.SmoothStep(0, 1, fixedOffsetProgress));
                }
                */
                //transform.position = player.transform.position + GetOffset() + currAccelerationOffset + fixedOffset;
                transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + GetOffset() /*+ currAccelerationOffset */, ref currentDampVelocity, 0.1f);

                transform.LookAt(GetTargetLookAt());
                break;

            case CameraModes.Fixed:
                transform.position = Vector3.SmoothDamp(transform.position, fixedPosition, ref currentVelocity, transitionFixedTime);
                transform.LookAt(player.transform.position);
                break;
        }
	}

    private void RotateCamera()
    {
        float rh = PlayerInput.Instance.Input.GetAxis("RightAnalogHoriz");
        float rv = PlayerInput.Instance.Input.GetAxis("RightAnalogVert"); 

        if (GameStatus.instance.InvertAxis)
        {
            rh *= -1;
            rv *= -1;
        }

        rotation.y += rh * rotationSpeed * Time.deltaTime;
        rotation.x += rv * rotationSpeed * Time.deltaTime;
    
        // Clamp rotation
        if (rotation.y > 360) rotation.y -= 360;
        else if(rotation.y < 0) rotation.y += 360;

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

        //Vector3 compoundRotation = rotation + new Vector3(cameraMode == 0 ? 0 : player.transform.eulerAngles.x, player.transform.eulerAngles.y, 0);

        Quaternion qRotation = new Quaternion();
        qRotation.eulerAngles = rotation;

        Quaternion playerRotation = new Quaternion();
        playerRotation.eulerAngles = new Vector3(cameraMode == 0 ? 0 : player.transform.eulerAngles.x, player.transform.eulerAngles.y, 0);

        //if (currentRotation.y - compoundRotation.y > 180) compoundRotation.y += 360;
        //else if (currentRotation.y - compoundRotation.y < -180) currentRotation.y += 360;

        //currentRotation = Vector3.SmoothDamp(currentRotation, compoundRotation, ref currentRotVelocity, rotationSmoothTime);
        currentRotation = playerRotation * qRotation;
    }
}
