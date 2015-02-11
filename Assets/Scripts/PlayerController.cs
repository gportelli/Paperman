using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    //private float startDelay = 1;

    private PlayerAerodynamicsController aerodynamics;
    private SparkleController sparkleController;
    private TurbineBoost turbineBoost;
    private WindBoost windBoost;

    public AudioSource AudioPick;

    private float stuckProgress = 100, stuckTimeout = 2;
    private float stuckTreshold = 0.05f;

    void Awake()
    {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
        sparkleController = GameObject.Find("SparkleRising").gameObject.GetComponent<SparkleController>();
        turbineBoost = GetComponent<TurbineBoost>();
        windBoost = GetComponent<WindBoost>();
    }

	void Start () {
        stuckProgress = 1;
	}

    void CheckStuck()
    {
        if (rigidbody.velocity.magnitude < stuckTreshold && windBoost.infiniteBoost == false && windBoost.GetAvailableBoost() <= 0)
            stuckProgress -= Time.deltaTime;
        else
            stuckProgress = stuckTimeout;

        if (stuckProgress <= 0)
        {
            stuckProgress = 0;
            GameController.instance.Gameover();
        }

    }

    public void EnableUserControl(bool value)
    {
        PlayerRotationController rotationController = GetComponent<PlayerRotationController>();

        rotationController.enabled = value;
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

        CheckStuck();
	}
   
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUp")
        {
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
            sparkleController.Play();

            AudioPick.Play();
        }
    }
}
