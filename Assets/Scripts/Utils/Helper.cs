
using UnityEngine;

class Helper
{
    static public float GetRotation(float angle)
    {
        if (angle > 180) return angle - 360;
        return angle;
    }

    static public Vector3 SmoothDampVector3(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed)
    {
        Vector3 result = new Vector3();

        result.x = Mathf.SmoothDamp(current.x, target.x, ref currentVelocity.x, smoothTime, maxSpeed);
        result.y = Mathf.SmoothDamp(current.y, target.y, ref currentVelocity.y, smoothTime, maxSpeed);
        result.z = Mathf.SmoothDamp(current.z, target.z, ref currentVelocity.z, smoothTime, maxSpeed);

        return result;
    }

    static public Vector3 SmoothDampVector3(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
    {
        Vector3 result = new Vector3();

        result.x = Mathf.SmoothDamp(current.x, target.x, ref currentVelocity.x, smoothTime);
        result.y = Mathf.SmoothDamp(current.y, target.y, ref currentVelocity.y, smoothTime);
        result.z = Mathf.SmoothDamp(current.z, target.z, ref currentVelocity.z, smoothTime);

        return result;
    }
}

