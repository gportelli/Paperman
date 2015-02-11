using UnityEngine;
using System.Collections;

public class SparkleController : MonoBehaviour {
    public float duration;
    private float progress;

    private ParticleEmitter sparkleParticles, sparkleParticlesSecondary;

    void Awake()
    {
        sparkleParticles = transform.Find("SparkleParticles").gameObject.GetComponent<ParticleEmitter>();
        sparkleParticlesSecondary = transform.Find("SparkleParticlesSecondary").gameObject.GetComponent<ParticleEmitter>();
    }

	// Use this for initialization
	void Start () {
        sparkleParticles.emit = false;
        sparkleParticlesSecondary.emit = false;
	}
	
	// Update is called once per frame
	void Update () {        
        if (progress > 0)
        {
            progress -= Time.deltaTime / duration;

            if (progress <= 0)
            {
                progress = 0;
                sparkleParticles.emit = false;
                sparkleParticlesSecondary.emit = false;
                
                //Debug.Log("end emitting...");
            }
        }
	}

    public void Play()
    {
        progress = duration;
        sparkleParticles.ClearParticles();
        sparkleParticles.emit = true;
        sparkleParticlesSecondary.emit = true;
        //Debug.Log("emitting...");
    }
}
