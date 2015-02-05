using UnityEngine;
using System.Collections;

public class PlayerRotationController : MonoBehaviour {
    public float X_Torque = 0.02f;
    public float Y_Torque = 0.025f;
    public float Z_Torque = 0.001f;

    public float MinXRot = -65f;
    public float MaxXRot = 65f;

    private float rotateX = 0, rotateY = 0;
    private float maxSpeed;

	void Start () {
        PlayerAerodynamicsController pac = GetComponent<PlayerAerodynamicsController>();
        maxSpeed = pac.getTerminalFrontSpeed();
	}
	
	void FixedUpdate () {
        GetUserInput();
        UpdateRotation();
	}

    void GetUserInput()
    {
        rotateX = Input.GetAxis("Vertical");
        rotateY = Input.GetAxis("Horizontal");
    }

    private void UpdateRotation()
    {
        if (rigidbody.velocity.magnitude < 0.1) return;

        float desiredZRotation = 0f, epsilon = 0.3f;
        float rx = Helper.GetRotation(transform.eulerAngles.x);
        //float ry = Helper.GetRotation(transform.eulerAngles.y);
        float rz = Helper.GetRotation(transform.eulerAngles.z);

        float minXRot = GetMinXRot();

        // Adjust x rotation
        if (rotateX >= 0 && rx > MaxXRot - epsilon)
        {
            rotateX = (MaxXRot - rx) * X_Torque;
        }
        else if (rotateX <= 0 && rx < minXRot + epsilon)
        {
            rotateX = (minXRot - rx) * X_Torque;
        }

        rigidbody.AddRelativeTorque(Vector3.right * rotateX * X_Torque);
        rigidbody.AddTorque(Vector3.up * rotateY * Y_Torque);

        if (Mathf.Abs(rotateY) > epsilon)
            desiredZRotation = -30 * Mathf.Sign(rotateY);

        if (Mathf.Abs(rz - desiredZRotation) > 1)
            rigidbody.AddRelativeTorque(Vector3.forward * -(rz - desiredZRotation) * Z_Torque);
    }

    private float GetMinXRot()
    {
        float v = Vector3.Dot(gameObject.rigidbody.velocity, transform.forward);

        return Mathf.Lerp(0, MinXRot, (v - maxSpeed / 5) * 4 / maxSpeed);
    }

}
