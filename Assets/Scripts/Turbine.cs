using UnityEngine;
using System.Collections;

public class Turbine : MonoBehaviour {
    public float RotationSpeed = 200;
    public float BoostMagnitude = 1;
    public float BoostDuration = 1;

    public Transform turbineA, turbineB;

    private TurbineBoost turbineBoost;

    void Awake()
    {
        turbineBoost = GameObject.FindGameObjectWithTag("Player").GetComponent<TurbineBoost>();
    }

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other) {
        if (other.tag == "PlayerCollider")
        {
            if(Vector3.Dot(turbineBoost.transform.forward, transform.forward) > 0)
                turbineBoost.SetBoost(transform.forward * BoostMagnitude, BoostDuration);
        }
    }

	void Update () {
        turbineA.Rotate(0, 0, Time.deltaTime * RotationSpeed);
        turbineB.Rotate(0, 0, -Time.deltaTime * RotationSpeed);
	}
}
