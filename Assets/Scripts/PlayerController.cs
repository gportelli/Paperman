using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private float startDelay = 1;

    public bool vsquare = false;

    /// <summary>
    /// The object mass
    /// </summary>
    public float mass = 1;

    /// <summary>
    /// The gravity acceleration
    /// </summary>
    public float g = 9.8f;

    /// <summary>
    /// The bottom surface drag coefficient
    /// </summary>
    public float cdBottom = 1f;

    /// <summary>
    /// The front area drag coefficient
    /// </summary>
    public float cdFront= 1f;

    /// <summary>
    /// The maximum angular speed
    /// </summary>
    public float maxAngularSpeed = 1f;

    /// <summary>
    /// The angular acceleration
    /// </summary>
    public float angularAcceleration = 1f;

    /// <summary>
    /// The current angular speed
    /// </summary>
    private float currentAngularSpeed;

    public bool showVectors;
    public float vectorsScale = 1f;

    public  GameObject redCubePrefab, greenCubePrefab, yellowCubePrefab, blueCubePrefab;
    private GameObject redCube, greenCube, yellowCube, blueCube;

    /// <summary>
    /// The velocity
    /// </summary>
    private Vector3 velocity;

    /// <summary>
    /// The result force
    /// </summary>
    private Vector3 resultForce;
    private Vector3 frictionBottom;
    private Vector3 frictionFront;

    /// <summary>
    /// Gets the instant acceleration.
    /// </summary>
    /// <returns></returns>
    public Vector3 getAcceleration()
    {
        return resultForce / mass;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 150, 10, 150, 30), "V=" + velocity.magnitude + " m/s^2");
    }

    // Use this for initialization
	void Start () {
        velocity = Vector3.zero;
        currentAngularSpeed = 0;

        redCube = Instantiate(redCubePrefab) as GameObject;
        greenCube = Instantiate(greenCubePrefab) as GameObject;
        yellowCube = Instantiate(yellowCubePrefab) as GameObject;
        blueCube = Instantiate(blueCubePrefab) as GameObject;

        //transform.Rotate(new Vector3(85.26318f, 0, 0));
        //velocity = new Vector3(0.0f, -9.5f, 2.3f);
	}

    public float getTerminalBottomSpeed()
    {
        return
            vsquare 
            ? Mathf.Sqrt(mass * g / cdBottom)
            : mass * g / cdBottom;
    }

    public float getTerminalFrontSpeed()
    {
        return 
            vsquare 
            ? Mathf.Sqrt(mass * g / cdFront)
            : mass * g / cdFront;
    }

    void FixedUpdate()
    {
        UpdateRotation();
        UpdateVelocity();
        UpdatePosition();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
            showVectors = !showVectors;

		UpdateVectors ();
	}

    /// <summary>
    /// Displays the force vectors.
    /// </summary>
	void UpdateVectors() {        
		redCube.renderer.enabled = showVectors;
        yellowCube.renderer.enabled = showVectors;
        blueCube.renderer.enabled = showVectors;
        greenCube.renderer.enabled = showVectors;

		if (showVectors)
		{
			DrawVector(frictionBottom, redCube, transform.TransformPoint(new Vector3(0, 0.2f, 0)));
			DrawVector(frictionFront, yellowCube, transform.TransformPoint(new Vector3(0, 0, -0.5f)));
			DrawVector(new Vector3(0, - mass * g, 0), blueCube, transform.position + new Vector3(0, -0.1f, 0));
			DrawVector(resultForce, greenCube, transform.position);
		}       
	}

    /// <summary>
    /// Updates the object position according to its velocity
    /// </summary>
    private void UpdatePosition()
    {
        transform.position += velocity * Time.deltaTime;

        // Wrap
        if (transform.position.y < 0)    transform.position = new Vector3(transform.position.x, 38, -18);
        if (transform.position.z > 20)  transform.position = new Vector3(transform.position.x, 38, -18);
        if (transform.position.z < -20) transform.position = new Vector3(transform.position.x, 38, 18);
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
    private void UpdateVelocity()
    {
        Vector3 windVelocity = -velocity;
        Vector3 weightForce = new Vector3(0, - mass * g, 0);
        Vector3 normalBottom = transform.up;
        Vector3 normalFront  = transform.forward;

        float windBottom = Vector3.Dot(normalBottom, windVelocity);
        float windFront = Vector3.Dot(normalFront, windVelocity);

        if (vsquare)
        {
            frictionBottom = cdBottom * normalBottom * windBottom * windBottom * Mathf.Sign(windBottom);
            frictionFront = cdFront * normalFront * windFront * windFront * Mathf.Sign(windFront);
        }
        else
        {
            frictionBottom = cdBottom * normalBottom * windBottom; // *windBottom * Mathf.Sign(windBottom);
            frictionFront = cdFront * normalFront * windFront;    // *windFront * Mathf.Sign(windFront);
        }        
       
        resultForce = weightForce + frictionFront + frictionBottom;

        //UpdateVectors();

        velocity = velocity + resultForce / mass * Time.deltaTime;

        //Debug.Log("rotation: " + transform.rotation.eulerAngles.x + " windVelocity:" + windVelocity + " frictionBottom:" + frictionBottom.z + " frictionFront" + frictionFront.z + " resultForce:" + resultForce.z);
    }

    /// <summary>
    /// Updates the object rotation according to the current angular speed and user inputs.
    /// </summary>
    private void UpdateRotation()
    {
        float w = Input.GetAxis("Vertical") * maxAngularSpeed;

        if (currentAngularSpeed != w)
        {
            if (Mathf.Abs(currentAngularSpeed - w) <= angularAcceleration * Time.deltaTime)
                currentAngularSpeed = w;
            else
            {
                if (currentAngularSpeed < w)
                    currentAngularSpeed += angularAcceleration * Time.deltaTime;
                else
                    currentAngularSpeed -= angularAcceleration * Time.deltaTime;
            }
        }

        transform.Rotate(new Vector3(currentAngularSpeed * Time.deltaTime, 0, 0));
    }
}
