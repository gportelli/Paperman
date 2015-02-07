using UnityEngine;
using System.Collections;

public class WindBoost : MonoBehaviour
{
    public float windSpeed = 4;
    public float windSmoothTime = 2;
    public float boostDuration = 1;
    public bool infiniteBoost = false;

    private Vector3 currWindSpeed, desiredWindSpeed, currVel;

    private Vector3 airfluxWind;

    private ParticleSystem boostWindParticle;
    private Color dustColor;

    private float boostLevel = 1;

    private Vector3 windVector;

    private bool boostIsPlaying = false;

    void Awake()
    {
        boostWindParticle = GameObject.Find("BoostWindParticles").gameObject.GetComponent<ParticleSystem>();
    }

    void Start()
    {
        dustColor = boostWindParticle.renderer.material.GetColor("_TintColor");
        airfluxWind = Vector3.zero;
        currVel = Vector3.zero;

        ShowEnergyLevel();
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

    void FixedUpdate()
    {        
        if (Input.GetButton("Fire1"))
            desiredWindSpeed = new Vector3(0, windSpeed, 0);          
        else
            desiredWindSpeed = Vector3.zero;        

        if (!infiniteBoost)
        {
            if (desiredWindSpeed != Vector3.zero)
            {
                boostLevel -= Time.deltaTime / boostDuration;
                if (boostLevel < 0)
                    boostLevel = 0;

                ShowEnergyLevel();
            }

            if (boostLevel == 0) desiredWindSpeed = Vector3.zero;
        }

        if (desiredWindSpeed == Vector3.zero && boostIsPlaying)
        {
            boostWindParticle.Stop();
            boostIsPlaying = false;
        }

        if (desiredWindSpeed != Vector3.zero && ! boostIsPlaying)
        {
            boostWindParticle.Play();
            boostIsPlaying = true;
        }

        currWindSpeed = Helper.SmoothDampVector3(currWindSpeed, desiredWindSpeed, ref currVel, windSmoothTime);

        Color c = new Color(dustColor.r, dustColor.g, dustColor.b, dustColor.a * currWindSpeed.magnitude / windSpeed);
        boostWindParticle.renderer.material.SetColor("_TintColor", c);

        windVector = currWindSpeed + airfluxWind; 
    }

    void ShowEnergyLevel()
    {
        Material m = renderer.materials[0];
        m.SetFloat("_EnergyLevel", 0.05f + boostLevel * 0.45f);
    }

    public Vector3 GetWindBoost()
    {
        return windVector;
    }
}
