using UnityEngine;
using System.Collections;

public class PlayerAerodynamicsController : MonoBehaviour {
    public bool vsquare = false;

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

    private Vector3 weightForce;
    
    private Vector3 frictionBottom;
    private Vector3 frictionFront;
    private Vector3 frictionRight;

    /// <summary>
    /// Gets the instant acceleration.
    /// </summary>
    /// <returns></returns>
    public Vector3 getAcceleration()
    {
        return (resultForce + weightForce) / rigidbody.mass;
    }

    // Use this for initialization
    void Start()
    {
        rigidbody.velocity = Vector3.zero;

        weightForce = rigidbody.mass * Physics.gravity;

        redCube = Instantiate(redCubePrefab) as GameObject;
        greenCube = Instantiate(greenCubePrefab) as GameObject;
        yellowCube = Instantiate(yellowCubePrefab) as GameObject;
        blueCube = Instantiate(blueCubePrefab) as GameObject;
    }

    public float getTerminalBottomSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(rigidbody.mass * Physics.gravity.magnitude / cdBottom)
            : rigidbody.mass * Physics.gravity.magnitude / cdBottom;
    }

    public float getTerminalFrontSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(rigidbody.mass * Physics.gravity.magnitude / cdFront)
            : rigidbody.mass * Physics.gravity.magnitude / cdFront;
    }

    public float getTerminalrightSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(rigidbody.mass * Physics.gravity.magnitude / cdRight)
            : rigidbody.mass * Physics.gravity.magnitude / cdRight;
    }

    public Vector3 GetVelocity()
    {
        return rigidbody.velocity;
    }

    void FixedUpdate()
    {
        UpdateForces();
    }

    // Update is called once per frame
    void Update()
    {
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

    /// <summary>
    /// Computes the sum of the forces (gravity and friction) and updates the velocity.
    /// </summary>
    private void UpdateForces()
    {
        Vector3 windVelocity = -rigidbody.velocity + windVector;

        Vector3 normalBottom = transform.up;
        Vector3 normalFront = transform.forward;
        Vector3 normalRight = transform.right;

        float windBottom = Vector3.Dot(normalBottom, windVelocity);
        float windFront = Vector3.Dot(normalFront, windVelocity);
        float windRight = Vector3.Dot(normalRight, windVelocity);

        if (vsquare)
        {
            frictionBottom = cdBottom * normalBottom * windBottom * windBottom * Mathf.Sign(windBottom);
            frictionFront = cdFront * normalFront * windFront * windFront * Mathf.Sign(windFront);
            frictionRight = cdRight * normalRight * windRight * windRight * Mathf.Sign(windRight);
        }
        else
        {
            frictionBottom = cdBottom * normalBottom * windBottom;
            frictionFront = cdFront * normalFront * windFront;
            frictionRight = cdRight * normalRight * windRight;
        }

        resultForce = frictionFront + frictionBottom + frictionRight;

        rigidbody.AddForce(resultForce);

        //Debug.Log("rotation: " + transform.rotation.eulerAngles.x + " windVelocity:" + windVelocity + " frictionBottom:" + frictionBottom.z + " frictionFront" + frictionFront.z + " resultForce:" + resultForce.z);
    }
}
