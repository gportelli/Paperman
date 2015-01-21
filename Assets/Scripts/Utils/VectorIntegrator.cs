using UnityEngine;
using System.Collections;

public class VectorIntegrator {
    int maxBufferLength = 10;
    int index;
    Vector3 [] buffer;
    float interval;

    public VectorIntegrator(float interval) {
        this.interval = interval;
        buffer = new Vector3[maxBufferLength];
        index = 0;
        for (int i = 0; i < maxBufferLength; i++) buffer[i] = Vector3.zero;
    }

    public void AddValue(Vector3 value)
    {
        buffer[index] = value;

        index = (index + 1) % maxBufferLength;
    }

    public Vector3 GetValue(float dt)
    {
        int length = (int)(interval / dt);
        if (length > maxBufferLength) length = maxBufferLength;
        if (length < 2) length = 2;

        int i = index - 1;
        if (i < 0) i += maxBufferLength;

        Vector3 result = Vector3.zero;
        
        for (int j = 0; j < length; j++)
            result += buffer[(i + j) % maxBufferLength];

        return result / length;
    }
}
