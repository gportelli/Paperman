using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
    GameObject player;

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 90, 10, 90, 60), "V=" + (int)(player.rigidbody.velocity.magnitude * 3.6) + " Km/h");
    }

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
