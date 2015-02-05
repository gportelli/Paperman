
using UnityEngine;

class Helper
{
    static public float GetRotation(float angle)
    {
        if (angle > 180) return angle - 360;
        return angle;
    }
}

