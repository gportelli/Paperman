using UnityEngine;
using System.Collections;

public class Anemometer : MonoBehaviour {
    public float PowerUpSize = 0.3f;
    private bool picked = false;
    private WindBoost windBoost;

    void Awake()
    {
        windBoost = GameObject.FindGameObjectWithTag("Player").GetComponent<WindBoost>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (picked) return;

        windBoost.IncreaseBoostLevel(PowerUpSize);

        picked = true;
    }
}
