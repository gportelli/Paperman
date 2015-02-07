using UnityEngine;
using System.Collections;

public class TurbineBoost : MonoBehaviour {
    private Vector3 currBoost;
    private float duration;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (duration > 0) duration -= Time.deltaTime;
        if (duration < 0) duration = 0;
	}

    public void SetBoost(Vector3 boostForce, float duration)
    {
        currBoost = boostForce;
        this.duration = duration;
    }

    public Vector3 GetBoost()
    {
        if (duration > 0)
            return currBoost;
        else
            return Vector3.zero;
    }
}
