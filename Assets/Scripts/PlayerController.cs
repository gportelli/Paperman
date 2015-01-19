using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public float mass = 1;
    public float gamma = 1;
    public float g = -9.8f;
    public float bottomArea = 1f;
    public float frontArea = 0.1f;
    public float maxAngularSpeed = 1f;
    public float angularAcceleration = 1f;
    private float currentAngularSpeed;

    public bool showVectors;
    public float vectorsScale = 1f;

    public  GameObject redCubePrefab, greenCubePrefab, yellowCubePrefab, blueCubePrefab;
    private GameObject redCube, greenCube, yellowCube, blueCube;

    private Vector3 velocity;
    private Vector3 resultForce;

    public Vector3 getAcceleration()
    {
        return resultForce / mass;
    }

    // Use this for initialization
	void Start () {
        velocity = Vector3.zero;
        currentAngularSpeed = 0;

        redCube = Instantiate(redCubePrefab) as GameObject;
        greenCube = Instantiate(greenCubePrefab) as GameObject;
        yellowCube = Instantiate(yellowCubePrefab) as GameObject;
        blueCube = Instantiate(blueCubePrefab) as GameObject;
	}

    void FixedUpdate()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
        UpdateRotation();
        UpdateVelocity();
        UpdatePosition();

		UpdateVectors ();
	}

	void UpdateVectors() {
		redCube.SetActive(showVectors);
		yellowCube.SetActive(showVectors);
		blueCube.SetActive(showVectors);
		greenCube.SetActive(showVectors);
		
		if (showVectors)
		{
			DrawVector(frictionBottom, redCube, transform.TransformPoint(new Vector3(0, 0.5f, 0)));
			DrawVector(frictionFront, yellowCube, transform.TransformPoint(new Vector3(0, 0, -1)));
			DrawVector(new Vector3(0, mass * g, 0), blueCube, transform.position + new Vector3(0, -0.5f, 0));
			DrawVector(resultForce, greenCube, transform.position);
		}       
	}

    private void UpdatePosition()
    {
        transform.position += velocity * Time.deltaTime;

        if (transform.position.y < 0)    transform.position = new Vector3(transform.position.x, 190, -90);
        if (transform.position.z > 100)  transform.position = new Vector3(transform.position.x, 190, -90);
        if (transform.position.z < -100) transform.position = new Vector3(transform.position.x, 190, 90);
    }

    [HideInInspector]
    public Vector3 frictionBottom;
    [HideInInspector]
    public Vector3 frictionFront;

    private void DrawVector(Vector3 v, GameObject cube, Vector3 vectorPosition)
    {
        if (v.magnitude < 0.1)
        {
            cube.SetActive(false);
            return;
        }
        else
            cube.SetActive(true);

        cube.transform.localScale = new Vector3(1, 1, v.magnitude / vectorsScale);
        cube.transform.position = vectorPosition;
        cube.transform.LookAt(cube.transform.position + v);
    }

    private void UpdateVelocity()
    {
        Vector3 windVelocity = -velocity;
        Vector3 weightForce = new Vector3(0, mass * g, 0);
        Vector3 bottomNormal = transform.TransformPoint(new Vector3(0, 1, 0)) - transform.position;
        Vector3 frontNormal  = transform.TransformPoint(new Vector3(0, 0, 1)) - transform.position;

        frictionBottom = bottomNormal * Vector3.Dot(bottomNormal, windVelocity) * bottomArea * gamma;
        frictionFront  = frontNormal  * Vector3.Dot(frontNormal, windVelocity)  * frontArea  * gamma;

        resultForce = weightForce + frictionFront + frictionBottom;

        velocity = velocity + resultForce / mass * Time.deltaTime;
    }

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
