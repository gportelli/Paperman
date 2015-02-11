using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {
    public bool IsRed = false;
    private bool picked = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (picked) return;

        //Debug.Log("Picked " + (IsRed?"Red" : "Yellow") + "...");

        GameController.instance.StarPicked(IsRed);

        picked = true;

        //if (IsRed) Debug.Break();
    }
}
