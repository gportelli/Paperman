using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //private float startDelay = 1;

    public int wrapWidth  = 20;
    public int wrapHeight = 40;

    public float windSpeed, windChangeDuration;
    private float currWindSpeed;
    private float currWindAcceleration;

    private PlayerAerodynamicsController aerodynamics;
    private PlayerRotationController rotationController;

    // Use this for initialization
	void Start () {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
        rotationController = GetComponent<PlayerRotationController>();

        float tmp = 0.001f;
        rigidbody.inertiaTensor = new Vector3(tmp, tmp, tmp);
	}

    public Vector3 GetVelocity()
    {
        return rigidbody.velocity;
    }

    public Vector3 getAcceleration()
    {
        return aerodynamics.getAcceleration();
    }

    void FixedUpdate()
    {
        WrapPosition();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire2"))
            aerodynamics.showVectors = !aerodynamics.showVectors;

        rotationController.rotateX = Input.GetAxis("Vertical");
        rotationController.rotateY = Input.GetAxis("Horizontal");

        UpdateWind();
	}

    void UpdateWind()
    {
        float destWindSpeed = 0;

        if (Input.GetButton("Fire1"))
            destWindSpeed = windSpeed;

        if (currWindSpeed < destWindSpeed) 
            currWindAcceleration = windSpeed / windChangeDuration;
        else 
            currWindAcceleration = -windSpeed / windChangeDuration;

        currWindSpeed += currWindAcceleration * Time.deltaTime;

        if ( (currWindAcceleration > 0 && currWindSpeed > destWindSpeed) ||
             (currWindAcceleration < 0 && currWindSpeed < destWindSpeed) )
        {
            currWindSpeed = destWindSpeed;
            currWindAcceleration = 0;
        }

        aerodynamics.windVector = new Vector3(0, currWindSpeed, 0);
    }

    /// <summary>
    /// Updates the object position according to its velocity
    /// </summary>
    private void WrapPosition()
    {
        // Wrap
        if (transform.position.y < 0)    transform.position = new Vector3(transform.position.x, wrapHeight-2, -wrapWidth + 2);
        if (transform.position.z > wrapWidth) transform.position = new Vector3(transform.position.x, wrapHeight - 2, -wrapWidth + 2);
        if (transform.position.z < -wrapWidth) transform.position = new Vector3(transform.position.x, wrapHeight - 2, wrapWidth - 2);
        if (transform.position.x > wrapWidth) transform.position = new Vector3(-wrapWidth + 2, wrapHeight - 2, transform.position.z);
        if (transform.position.x < -wrapWidth) transform.position = new Vector3(wrapWidth - 2, wrapHeight - 2, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Cube")
            Destroy(other.gameObject);
    }
}
