using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public Vector3 cameraOffset;
    public float accelerationSensitivity = 0.1f;
    public float transitionTime = 0.5f;
    public float maxOffset = 10;

    private Vector3 accelerationOffset;
    private Vector3 currAccelerationOffset;

    private GameObject player;
    private PlayerController playerController;

    private VectorIntegrator vectorIntegrator;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>() as PlayerController;

        currAccelerationOffset = accelerationOffset = Vector3.zero;

		gameObject.transform.position = player.transform.position  + cameraOffset;

        vectorIntegrator = new VectorIntegrator(0.2f);
	}
	
	// Update is called once per frame
	void Update () {
        vectorIntegrator.AddValue(-playerController.getAcceleration());

        accelerationOffset = vectorIntegrator.GetValue(Time.deltaTime) * accelerationSensitivity;
        
        if (accelerationOffset.magnitude > maxOffset)
            accelerationOffset = accelerationOffset.normalized * maxOffset;

        float step = Time.deltaTime / transitionTime;
        Vector3 delta = accelerationOffset - currAccelerationOffset;

        if (delta.magnitude > step)
            currAccelerationOffset += (accelerationOffset - currAccelerationOffset).normalized * step;
        else
            currAccelerationOffset = accelerationOffset;

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);

		gameObject.transform.position = player.transform.position + q * cameraOffset + currAccelerationOffset;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, player.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
	}
}
