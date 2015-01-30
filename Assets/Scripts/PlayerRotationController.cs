using UnityEngine;
using System.Collections;

public class PlayerRotationController : MonoBehaviour {
    public bool rotationYZ = false;

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
        Vector3 rotation = transform.eulerAngles;

        float w = Input.GetAxis("Vertical") * maxAngularSpeedX;

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

        rotation.x += currentAngularSpeed * Time.deltaTime;
        if (rotation.x > 360) rotation.x -= 360;
        else if (rotation.x < 0) rotation.x += 360;

        transform.Rotate(new Vector3(currentAngularSpeed * Time.deltaTime, 0, 0), Space.World);

        if (rotationYZ)
        {
            w = Input.GetAxis("Horizontal");
            rotation.z = -w * 30;

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

            rotation.y += currentAngularSpeedY * Time.deltaTime;

            if (rotation.y > 360) rotation.y -= 360;
            else if (rotation.y < 0) rotation.y += 360;
        }

        transform.eulerAngles = rotation;
    }
}
