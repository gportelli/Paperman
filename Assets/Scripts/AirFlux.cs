using UnityEngine;
using System.Collections;

public class AirFlux : MonoBehaviour {
    public float Height = 1f;
    public float Radius = 1f;
    public float Density = 10f;
    public float WindVelocity = 1f;

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().renderer.enabled = false;
	}

    public void UpdateParticlesParameters()
    {
        transform.localScale = new Vector3(Radius, Height, Radius);

        ParticleSystem ps = transform.Find("AirFluxParticles").GetComponent<ParticleSystem>();

        ps.startLifetime = Height * 1.7f / 3f / WindVelocity;
        ps.maxParticles = (int)(Density * Height * Radius * Radius);
        ps.transform.localScale = new Vector3(1, 1, 1);
        ps.startSpeed = 1.8f * WindVelocity;
        ps.emissionRate = 4 * WindVelocity;

        CapsuleCollider collider = GetComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 0.5f - Radius / Height / 2, 0);
        collider.height = 1 + Radius / Height;
    }

    public Vector3 GetWindVector()
    {
        return transform.TransformDirection(new Vector3(0, WindVelocity, 0));
    }

	// Update is called once per frame
	void Update () {
	
	}
}
