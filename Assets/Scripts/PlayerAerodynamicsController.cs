using UnityEngine;
using System.Collections;

public class PlayerAerodynamicsController : MonoBehaviour {
    public bool vsquare = false;

    public float Gravity = 9.81f;
    public float Mass    = 0.05f;

    /// <summary>
    /// The bottom surface drag coefficient
    /// </summary>
    public float cdBottom = 1f;

    /// <summary>
    /// The front area drag coefficient
    /// </summary>
    public float cdFront = 1f;

    /// <summary>
    /// The left area drag coefficient
    /// </summary>
    public float cdRight = 1f;

    public float LiftCoefficient;

    public  float maxForce = 1f;

    public float cdFrontZeroStart = 0f;  // Start of the x rotation range (degrees) where cdFront is linear interpolated to zero (positive x up)
    public float cdFrontZeroSize = 30f; // Size of the x rotation range (degrees) where cdFront is linear interpolated to zero (positive x up)

    public bool showVectors;
    public float vectorsScale = 1f;

    [HideInInspector]
    public Vector3 windVector;

    public GameObject redCubePrefab, greenCubePrefab, yellowCubePrefab, blueCubePrefab;

    private GameObject redCube, greenCube, yellowCube, blueCube;

    /// <summary>
    /// The velocity
    /// </summary>
    //private Vector3 velocity;

    /// <summary>
    /// The result force
    /// </summary>
    private Vector3 resultForce;

    private Vector3 frictionBottom;
    private Vector3 frictionFront;
    private Vector3 frictionRight;
    private Vector3 lift;
    private Vector3 lastVelocity;

    private Vector3 ClampForce(Vector3 force)
    {       
        if (force.magnitude > maxForce)
        {
            //Debug.Log("Clamping force=" + force.magnitude);

            return force.normalized * maxForce;
        }
        else
            return force;
    }

    /// <summary>
    /// Gets the instant acceleration.
    /// </summary>
    /// <returns></returns>
    public Vector3 getAcceleration()
    {
        return (rigidbody.velocity - lastVelocity) / Time.deltaTime;

        //return (resultForce + weightForce) / rigidbody.mass;
    }

    // Use this for initialization
    void Start()
    {
        rigidbody.velocity = Vector3.zero;

        Physics.gravity = new Vector3(0, -Gravity, 0);
        rigidbody.mass = Mass;

        float tmp = rigidbody.mass / 250;
        rigidbody.inertiaTensor = new Vector3(tmp, tmp, tmp);
        rigidbody.centerOfMass = new Vector3(0, 0, 0);

        redCube = Instantiate(redCubePrefab) as GameObject;
        greenCube = Instantiate(greenCubePrefab) as GameObject;
        yellowCube = Instantiate(yellowCubePrefab) as GameObject;
        blueCube = Instantiate(blueCubePrefab) as GameObject;

        //rigidbody.velocity = new Vector3(0, -10f, 0);
    }

    public float getTerminalBottomSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(Mass * Gravity / cdBottom)
            : Mass * Gravity / cdBottom;
    }

    public float getTerminalFrontSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(Mass * Gravity / cdFront)
            : Mass * Gravity / cdFront;
    }

    public float getTerminalrightSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(Mass * Gravity / cdRight)
            : Mass * Gravity / cdRight;
    }

    public Vector3 GetVelocity()
    {
        return rigidbody.velocity;
    }

    void FixedUpdate()
    {
        //if (count-- == 0) Debug.Break();

        UpdateForces();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
            showVectors = !showVectors;

        UpdateVectors();
    }

    /// <summary>
    /// Displays the force vectors.
    /// </summary>
    void UpdateVectors()
    {
        redCube.renderer.enabled = showVectors;
        yellowCube.renderer.enabled = showVectors;
        blueCube.renderer.enabled = showVectors;
        greenCube.renderer.enabled = showVectors;

        if (showVectors)
        {
            Vector3 weightForce = new Vector3(0, -rigidbody.mass * Physics.gravity.magnitude, 0);

            DrawVector(frictionBottom, redCube, transform.TransformPoint(new Vector3(0, 0.2f, 0)));
            DrawVector(frictionFront, yellowCube, transform.TransformPoint(new Vector3(0, 0, -0.5f)));
            DrawVector(new Vector3(0, -rigidbody.mass * Physics.gravity.magnitude, 0), blueCube, transform.position + new Vector3(0, -0.1f, 0));
            DrawVector(resultForce + weightForce, greenCube, transform.position);
            DrawVector(lift, yellowCube, transform.TransformPoint(new Vector3(0.05f, 0.2f, 0)));
        }
    }

    /// <summary>
    /// Draws the vector.
    /// </summary>
    /// <param name="v">The vector.</param>
    /// <param name="cube">The cube wich represents the vector.</param>
    /// <param name="vectorPosition">The vector position.</param>
    private void DrawVector(Vector3 v, GameObject cube, Vector3 vectorPosition)
    {
        if (v.magnitude * vectorsScale < 0.01)
        {
            cube.SetActive(false);
            return;
        }
        else
            cube.SetActive(true);

        cube.transform.localScale = new Vector3(.2f, .2f, v.magnitude * vectorsScale);
        cube.transform.position = vectorPosition;
        cube.transform.LookAt(cube.transform.position + v);
    }

    private float lastV1, lastV2;

    /// <summary>
    /// Computes the sum of the forces (gravity and friction) and updates the velocity.
    /// </summary>
    private void UpdateForces()
    {
        lastVelocity = rigidbody.velocity;

        Vector3 windVelocity = -rigidbody.velocity + windVector;

        Vector3 normalBottom = transform.up;
        Vector3 normalFront = transform.forward;
        Vector3 normalRight = transform.right;

        float windBottom = Vector3.Dot(normalBottom, windVelocity);
        float windFront = Vector3.Dot(normalFront, windVelocity);
        float windRight = Vector3.Dot(normalRight, windVelocity);

        float currCdFront = GetCdFront();

        if (vsquare)
        {
            frictionBottom = cdBottom * normalBottom * windBottom * windBottom * Mathf.Sign(windBottom);
            frictionFront = currCdFront * normalFront * windFront * windFront * Mathf.Sign(windFront);
            frictionRight = cdRight * normalRight * windRight * windRight * Mathf.Sign(windRight);
        }
        else
        {
            frictionBottom = cdBottom * normalBottom * windBottom;
            frictionFront = currCdFront * normalFront * windFront;
            frictionRight = cdRight * normalRight * windRight;
        }

        lift = GetLift(windFront);
        
        resultForce = ClampForce(frictionFront) + ClampForce(frictionBottom) + ClampForce(frictionRight) + ClampForce(lift);
        /*
        if(Mathf.Sign(lastV2 - lastV1) != Mathf.Sign(lastV1 - rigidbody.velocity.y))
            Debug.LogWarning(resultForce.y + " | " + lastV2 + " " + lastV1 + " " +rigidbody.velocity.y);

        //Debug.Log(resultForce.y + " | " + rigidbody.velocity.y);

        lastV2 = lastV1;
        lastV1 = rigidbody.velocity.y;
        */

        rigidbody.AddForce(resultForce);

        //Debug.Log("rotation: " + transform.rotation.eulerAngles.x + " windVelocity:" + windVelocity + " frictionBottom:" + frictionBottom.z + " frictionFront" + frictionFront.z + " resultForce:" + resultForce.z);
    }

    private float GetCdFront()
    {
        float rotX = Helper.GetRotation(transform.eulerAngles.x + cdFrontZeroStart);

        if (rotX > 0) return cdFront;
        else return Mathf.Lerp(cdFront, 0, -rotX / cdFrontZeroSize);
    }

    private Vector3 GetLift(float windVelocity)
    {
        if (windVelocity > 0) return Vector3.zero;

        return Mathf.Cos(transform.eulerAngles.x * Mathf.Deg2Rad) * LiftCoefficient * windVelocity * windVelocity * transform.up;
    }
}
