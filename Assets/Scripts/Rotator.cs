using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
    public float RotationSpeed = 400;
    public float OscillationSize = 0.1f;
    public float OscillationPeriod = 3;

    private float progress;
    private Vector3 position;

	// Use this for initialization
	void Start () {
        position = transform.position;

        progress = Random.Range(0f, Mathf.PI * 2);
	}
	
	// Update is called once per frame
	void Update () {
        progress += Time.deltaTime / OscillationPeriod * Mathf.PI * 2;
        if (progress > 2 * Mathf.PI)
            progress -= 2 * Mathf.PI;

        transform.Rotate(new Vector3(0, RotationSpeed * Time.deltaTime, 0));
        transform.position = position + new Vector3(0, Mathf.Sin(progress) * OscillationSize, 0);
	}
}
