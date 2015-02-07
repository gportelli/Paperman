using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //private float startDelay = 1;

    private PlayerAerodynamicsController aerodynamics;
    private SparkleController sparkleController;
    private TurbineBoost turbineBoost;

    void Awake()
    {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
        sparkleController = GameObject.Find("SparkleRising").gameObject.GetComponent<SparkleController>();
        turbineBoost = GetComponent<TurbineBoost>();
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

	void FixedUpdate () {
        rigidbody.AddForce(turbineBoost.GetBoost());
	}
   
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        if (other.tag == "Cube")
        {
            Destroy(other.gameObject);
            sparkleController.Play();
        }
    }
}
