using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
    public Vector3 offset;
    public Transform target;

	void Awake () {
        
	}

    void Start()
    {

    }
	
	// Update is called once per frame
	void LateUpdate () {
        transform.position = target.position + offset;
	}
}
