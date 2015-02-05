using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //private float startDelay = 1;

    private PlayerAerodynamicsController aerodynamics;
    private SparkleController sparkleController;


    void Awake()
    {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
        sparkleController = GameObject.Find("SparkleRising").gameObject.GetComponent<SparkleController>();
    }

	void Start () {        
        
	}

    public Vector3 GetVelocity()
    {
        return rigidbody.velocity;
    }

    public Vector3 getAcceleration()
    {
        return aerodynamics.getAcceleration();
    }

	// Update is called once per frame
	void Update () {
        
	}
   
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cube")
        {
            Destroy(other.gameObject);
            sparkleController.Play();
        }
    }
}
