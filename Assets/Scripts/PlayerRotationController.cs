using UnityEngine;
using System.Collections;

public class PlayerRotationController : MonoBehaviour {
    public bool rotationYZ = false;

    [HideInInspector]
    public float rotateX = 0, rotateY = 0;

    /// <summary>
    /// The maximum angular speed
    /// </summary>
    public float maxAngularSpeedX = 1f;
    public float maxAngularSpeedY = 1f;

    /// <summary>
    /// The angular acceleration
    /// </summary>
    public float angularAccelerationX = 1f;
    public float angularAccelerationY = 1f;

    /// <summary>
    /// The current angular speed
    /// </summary>
    private float currentAngularSpeed;
    private float currentAngularSpeedY;

    private float destZRotation;

	// Use this for initialization
	void Start () {
        currentAngularSpeed = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        UpdateRotation();
	}

    private void UpdateRotation()
    {
        if (rigidbody.velocity.magnitude < 0.1) return;

        float w;
        
        w = rotateX * maxAngularSpeedX;
        rigidbody.AddRelativeTorque(Vector3.right * w);

        w = rotateY * maxAngularSpeedY;
        //rigidbody.AddRelativeTorque(-Vector3.forward * w);
        rigidbody.AddTorque(Vector3.up * w);

        if (Mathf.Abs(rotateY) < 0.3)
            destZRotation = 0;
        else if (w > 0)
            destZRotation = -30;
        else
            destZRotation = 30;

        float rz = transform.eulerAngles.z;
        if (rz > 180) rz -= 360;
        if (Mathf.Abs(rz - destZRotation) > 1)
        {
            rigidbody.AddRelativeTorque(Vector3.forward * -(rz - destZRotation) * 0.005f);
        }

        /*
        if (currentAngularSpeed != w)
        {
            if (Mathf.Abs(currentAngularSpeed - w) <= angularAccelerationX * Time.deltaTime)
                currentAngularSpeed = w;
            else
            {
                if (currentAngularSpeed < w)
                    currentAngularSpeed += angularAccelerationX * Time.deltaTime;
                else
                    currentAngularSpeed -= angularAccelerationX * Time.deltaTime;
            }
        }

        transform.Rotate(new Vector3(currentAngularSpeed * Time.deltaTime, 0, 0), Space.World);
        
        if (rotationYZ)
        {
            w = rotateY;
            //rotation.z = -w * 30;

            w *= maxAngularSpeedY;

            if (currentAngularSpeedY != w)
            {
                if (Mathf.Abs(currentAngularSpeedY - w) <= angularAccelerationY * Time.deltaTime)
                    currentAngularSpeedY = w;
                else
                {
                    if (currentAngularSpeedY < w)
                        currentAngularSpeedY += angularAccelerationY * Time.deltaTime;
                    else
                        currentAngularSpeedY -= angularAccelerationY * Time.deltaTime;
                }
            }

            //rotation.y += currentAngularSpeedY * Time.deltaTime;

            //if (rotation.y > 360) rotation.y -= 360;
            //else if (rotation.y < 0) rotation.y += 360;

            transform.Rotate(new Vector3(0, currentAngularSpeedY * Time.deltaTime, 0), Space.World);
        }
         * */
    }
}
