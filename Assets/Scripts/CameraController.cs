using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public int cameraMode = 0;

    public Vector3 cameraOffset;
    public float cameraDistance = 1;
    public float cameraHeight = 1;
    public float accelerationSensitivity = 0.1f;
    public float transitionTime = 1f;
    public float maxOffset = 10;

    private Vector3 accelerationOffset;
    private Vector3 currAccelerationOffset;

    private GameObject player;
    private PlayerController playerController;

    private Vector3 currentVelocity;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>() as PlayerController;

        currAccelerationOffset = accelerationOffset = Vector3.zero;

		gameObject.transform.position = player.transform.position  + cameraOffset;
	}

	// Update is called once per frame
	void LateUpdate () {
        //vectorIntegrator.AddValue(-playerController.getAcceleration());
        //accelerationOffset = vectorIntegrator.GetValue(Time.deltaTime) * accelerationSensitivity;

        accelerationOffset = -playerController.getAcceleration() * accelerationSensitivity;

        if (accelerationOffset.magnitude > maxOffset)
            accelerationOffset = accelerationOffset.normalized * maxOffset;

        currAccelerationOffset = Helper.SmoothDampVector3(currAccelerationOffset, accelerationOffset, ref currentVelocity, transitionTime);

        Quaternion q = new Quaternion();

        if(cameraMode == 0)
        {
            gameObject.transform.position = player.transform.position + cameraOffset + currAccelerationOffset;
        }
        else if (cameraMode == 1)
        {
            q.eulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);

            gameObject.transform.position = player.transform.position + q * cameraOffset + currAccelerationOffset;
            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, player.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        }
        else if (cameraMode == 2)
        {
            Vector3 offset = new Vector3(0, cameraHeight, cameraDistance);

            Vector3 vel = playerController.GetVelocity();

            if (vel.magnitude > 0)
            {
                Vector3 velYplane = new Vector3(vel.x, 0, vel.z);
                //if (vel.y / velYplane.magnitude < 0.2)
                //{
                    q = Quaternion.LookRotation(-velYplane);
                //}
            }
            
            gameObject.transform.position = player.transform.position + q * offset + currAccelerationOffset;

            gameObject.transform.eulerAngles = 
                new Vector3(
                    gameObject.transform.eulerAngles.x,
                    Quaternion.LookRotation(player.transform.position - gameObject.transform.position).eulerAngles.y,
                    gameObject.transform.eulerAngles.z);
        }
        else if (cameraMode == 3)
        {
            gameObject.transform.position = player.transform.position + cameraOffset;
        }
	}
}
