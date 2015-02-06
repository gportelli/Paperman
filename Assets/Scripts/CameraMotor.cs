using UnityEngine;
using System.Collections;

public class CameraMotor : MonoBehaviour {
    public float speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float vx = Input.GetAxis("Horizontal") * speed;
        float vy = Input.GetAxis("Vertical") * speed;

        transform.Translate(new Vector3(vx * Time.deltaTime, 0, vy * Time.deltaTime));
	}
}
