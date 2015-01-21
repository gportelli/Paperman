using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private float startDelay = 1;

    public bool vsquare  = false;
    public bool rotationYZ = false;

    public int wrapWidth  = 20;
    public int wrapHeight = 40;

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
    /// The left area drag coefficient
    /// </summary>
    public float cdRight = 1f;

    /// <summary>
    /// The maximum angular speed
    /// </summary>
    public float maxAngularSpeed = 1f;
    public float maxAngularSpeedY = 1f;

    /// <summary>
    /// The angular acceleration
    /// </summary>
    public float angularAcceleration = 1f;

    public float angularAccelerationY = 1f;

    /// <summary>
    /// The current angular speed
    /// </summary>
    private float currentAngularSpeed;
    private float currentAngularSpeedY;

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
    private Vector3 frictionRight;

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
        GUI.Label(new Rect(Screen.width - 150, 10, 150, 30), "V=" + (int)(velocity.magnitude * 3.6) + " Km/h");
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

    public float getTerminalrightSpeed()
    {
        return
            vsquare
            ? Mathf.Sqrt(mass * g / cdRight)
            : mass * g / cdRight;
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
        if (transform.position.y < 0)    transform.position = new Vector3(transform.position.x, wrapHeight-2, -wrapWidth + 2);
        if (transform.position.z > wrapWidth) transform.position = new Vector3(transform.position.x, wrapHeight - 2, -wrapWidth + 2);
        if (transform.position.z < -wrapWidth) transform.position = new Vector3(transform.position.x, wrapHeight - 2, wrapWidth - 2);
        if (transform.position.x > wrapWidth) transform.position = new Vector3(-wrapWidth + 2, wrapHeight - 2, transform.position.z);
        if (transform.position.x < -wrapWidth) transform.position = new Vector3(wrapWidth - 2, wrapHeight - 2, transform.position.z);
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
        Vector3 normalRight   = transform.right;

        float windBottom = Vector3.Dot(normalBottom, windVelocity);
        float windFront = Vector3.Dot(normalFront, windVelocity);
        float windRight = Vector3.Dot(normalRight, windVelocity);

        if (vsquare)
        {
            frictionBottom = cdBottom * normalBottom * windBottom * windBottom * Mathf.Sign(windBottom);
            frictionFront  = cdFront  * normalFront  * windFront * windFront * Mathf.Sign(windFront);
            frictionRight = cdRight * normalRight * windRight * windRight * Mathf.Sign(windRight);
        }
        else
        {
            frictionBottom = cdBottom * normalBottom * windBottom;
            frictionFront = cdFront * normalFront * windFront;
            frictionRight = cdRight * normalRight * windRight;
        }        
       
        resultForce = weightForce + frictionFront + frictionBottom + frictionRight;

        //UpdateVectors();

        velocity = velocity + resultForce / mass * Time.deltaTime;

        //Debug.Log("rotation: " + transform.rotation.eulerAngles.x + " windVelocity:" + windVelocity + " frictionBottom:" + frictionBottom.z + " frictionFront" + frictionFront.z + " resultForce:" + resultForce.z);
    }

    private float rotationX = 0;
    private float rotationY = 0;
    private float rotationZ = 0;

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

        rotationX += currentAngularSpeed * Time.deltaTime;
        if (rotationX > 360) rotationX -= 360;
        else if (rotationX < 0) rotationX += 360;

        transform.Rotate(new Vector3(currentAngularSpeed * Time.deltaTime, 0, 0), Space.World);

        if (rotationYZ)
        {
            w = Input.GetAxis("Horizontal");
            rotationZ = -w * 30;

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

            rotationY += currentAngularSpeedY * Time.deltaTime;

            if (rotationY > 360) rotationY -= 360;
            else if (rotationY < 0) rotationY += 360;
        }

        transform.eulerAngles = new Vector3(rotationX, rotationY, rotationZ);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cube")
            Destroy(other.gameObject);
    }
}
