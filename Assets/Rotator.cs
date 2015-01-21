using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(30 * Time.deltaTime, 50 * Time.deltaTime, 90 * Time.deltaTime));
	}
}
