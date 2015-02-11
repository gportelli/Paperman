using UnityEngine;
using System.Collections;

public class TurbineActivator : MonoBehaviour {
    public float RotationSpeed = 200;
    public Transform turbineA, turbineB;
    public Turbine[] ConnectedTurbines;
    private bool picked = false;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < ConnectedTurbines.Length; i++)
            ConnectedTurbines[i].Off();
	}

    void Update()
    {
        turbineA.Rotate(0, 0, Time.deltaTime * RotationSpeed);
        turbineB.Rotate(0, 0, -Time.deltaTime * RotationSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (picked) return;

        for (int i = 0; i < ConnectedTurbines.Length; i++)
            ConnectedTurbines[i].On();

        picked = true;
    }
}
