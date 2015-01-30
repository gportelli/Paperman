using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //private float startDelay = 1;

    public int wrapWidth  = 20;
    public int wrapHeight = 40;

    private PlayerAerodynamicsController aerodynamics;

    // Use this for initialization
	void Start () {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
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
        if (Input.GetButtonDown("Fire1"))
            aerodynamics.showVectors = !aerodynamics.showVectors;
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
