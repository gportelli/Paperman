using UnityEngine;
using System.Collections;

public class WindBoost : MonoBehaviour
{
    public float windSpeed = 4;
    public float windSmoothTime = 2;

    private Vector3 currWindSpeed, desiredWindSpeed, currVel;

    private Vector3 airfluxWind;

    private PlayerAerodynamicsController aerodynamics;
    private ParticleSystem boostWindParticle;
    private Color dustColor;

    void Awake()
    {
        aerodynamics = GetComponent<PlayerAerodynamicsController>();
        boostWindParticle = GameObject.Find("BoostWindParticles").gameObject.GetComponent<ParticleSystem>();
    }

    void Start()
    {
        dustColor = boostWindParticle.renderer.material.GetColor("_TintColor");
        airfluxWind = Vector3.zero;
        currVel = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "AirFlux") return;

        airfluxWind = other.GetComponent<AirFlux>().GetWindVector();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "AirFlux") return;

        airfluxWind = Vector3.zero;
    }

    void Update()
    {        
        if (Input.GetButtonDown("Fire1"))
        {
            desiredWindSpeed = new Vector3(0, windSpeed, 0);

            boostWindParticle.Stop();
            boostWindParticle.Play();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            desiredWindSpeed = Vector3.zero;

            boostWindParticle.Stop();
        }

        currWindSpeed = Helper.SmoothDampVector3(currWindSpeed, desiredWindSpeed, ref currVel, windSmoothTime);

        Color c = new Color(dustColor.r, dustColor.g, dustColor.b, dustColor.a * currWindSpeed.magnitude / windSpeed);
        boostWindParticle.renderer.material.SetColor("_TintColor", c);

        aerodynamics.windVector = currWindSpeed + airfluxWind; 
    }
}
