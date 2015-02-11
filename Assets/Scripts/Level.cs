using UnityEngine;
using System.Collections;
using AClockworkBerry.Unity.Splines;

[System.Serializable]
public class Level {
    public GameObject LevelObject;
    public Window StartWindow, EndWindow;
    public Spline CamPath, CamLookAtPath;
    public float IntroDuration = 10f;
    
    // Boost
    public bool InfiniteBoost = false;
    public float BoostDuration = 5;
    public float StartBoost = 1;

    // Aerodynamics
    public bool OverrideDynamics = false;
    public float Gravity = 0;
    public float Mass = 0;
    public float cdBottom = 0;
    public float cdFront = 0;
    public float cdRight = 0;
    public float LiftCoefficient = 0;
}
