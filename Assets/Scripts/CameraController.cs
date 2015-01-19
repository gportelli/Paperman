using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public Vector3 cameraOffset;
    public float accelerationSensitivity = 0.1f;
    public float k = 5;
    public float maxOffset = 10;

    private Vector3 accelerationOffset;
    private Vector3 currAccelerationOffset;

    private GameObject player;
    private PlayerController playerController;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>() as PlayerController;

        currAccelerationOffset = accelerationOffset = Vector3.zero;

		gameObject.transform.position = player.transform.position  + cameraOffset;
	}
	
	// Update is called once per frame
	void Update () {
		//return;

        accelerationOffset = - playerController.getAcceleration() * accelerationSensitivity;
        
        if (accelerationOffset.magnitude > maxOffset)
            accelerationOffset = accelerationOffset.normalized * maxOffset;

        currAccelerationOffset += (accelerationOffset - currAccelerationOffset) * Time.deltaTime * k;

		gameObject.transform.position = player.transform.position + cameraOffset + currAccelerationOffset;
	}
}
