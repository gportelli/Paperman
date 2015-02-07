using UnityEngine;
using System.Collections;

public class PlayerRotationController : MonoBehaviour {
    public float X_Torque = 0.02f;
    public float Y_Torque = 0.025f;
    public float Z_Torque = 0.001f;

    // x rotation range (negative up!)
    public float MinXRot = -65f;
    public float MaxXRot = 65f;

    public float turbulenceMagnitudeX = 1f;
    public float turbulenceMagnitudeZ = 10f;
    private float xTurbulence, xTurbulencePeriod, zTurbulence, zTurbulencePeriod;

    private float rotateX = 0, rotateY = 0;
    private float terminalFrontSpeed;

    private PlayerAerodynamicsController aerodynamics;

	void Start () {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
        terminalFrontSpeed = aerodynamics.getTerminalFrontSpeed();
	}
	
	void FixedUpdate () {
        GetUserInput();
        UpdateRotation();
	}

    //public float testSpeed = 0;

    float GetZTurbulence(float forwardWindSpeed)
    {
        float v = Mathf.Clamp01(Mathf.Abs(forwardWindSpeed /terminalFrontSpeed) - aerodynamics.windVector.magnitude);
        //v = testSpeed;
        
        if (v > 0.8) return 0;

        if (zTurbulence == 0)
            zTurbulencePeriod = 0.5f + Random.Range(0f, 1f) * 3 * (1 - v);

        zTurbulence += Time.deltaTime * Mathf.PI * 2 / zTurbulencePeriod;
        if (zTurbulence > 2 * Mathf.PI) zTurbulence = 0;

        float magnitude = turbulenceMagnitudeZ * (1 - v);
        float t = Mathf.Sin(zTurbulence) * magnitude;

        //Debug.Log(t);

        return t;
    }

    float GetXTurbulence(float forwardWindSpeed)
    {
        float v = Mathf.Clamp01(Mathf.Abs(forwardWindSpeed / terminalFrontSpeed) - aerodynamics.windVector.magnitude);
        //v = testSpeed;

        if (v > 0.8) return 0;

        if (xTurbulence == 0)
            xTurbulencePeriod = 0.5f + Random.Range(0f, 1f) * 3 * (1 - v);

        xTurbulence += Time.deltaTime * Mathf.PI * 2 / xTurbulencePeriod;
        if (xTurbulence > 2 * Mathf.PI) xTurbulence = 0;

        float magnitude = turbulenceMagnitudeX * (1 - v);
        float t = Mathf.Sin(xTurbulence) * magnitude;

        //Debug.Log(t);

        return t;
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

        // Adjust x rotation
        float minXRot = GetMinXRot();
        
        if (rotateX >= 0 && rx > MaxXRot - epsilon)
        {
            rotateX = (MaxXRot - rx) * X_Torque;
        }
        else if (rotateX <= 0 && rx < minXRot + epsilon)
        {
            rotateX = (minXRot - rx) * X_Torque;
        }

        float forwardWindSpeed = Vector3.Dot(rigidbody.velocity, transform.forward);

        rigidbody.AddRelativeTorque(Vector3.right * (rotateX + GetXTurbulence(forwardWindSpeed)) * X_Torque);
        rigidbody.AddTorque(Vector3.up * rotateY * Y_Torque);
        
        if (Mathf.Abs(rotateY) > epsilon)
            desiredZRotation = -30 * Mathf.Sign(rotateY);

        desiredZRotation += GetZTurbulence(forwardWindSpeed);

        if (Mathf.Abs(rz - desiredZRotation) > 1)
            rigidbody.AddRelativeTorque(Vector3.forward * -(rz - desiredZRotation) * Z_Torque);
         
    }

    // stall when front velocity reaches zero
    private float GetMinXRot()
    {
        float frontVelocity = Vector3.Dot(gameObject.rigidbody.velocity, transform.forward);
        
        float k;
        //k = (frontVelocity - terminalFrontSpeed / 5) * 4 / terminalFrontSpeed;
        k = frontVelocity / terminalFrontSpeed * 4;

        return Mathf.Lerp(0, MinXRot, k);
    }

}
