using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AirFlux)), CanEditMultipleObjects]
public class AirFluxInspector : Editor
{
	public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Height"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Radius"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Density"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("WindVelocity"));
        serializedObject.ApplyModifiedProperties();

        foreach (Object o in serializedObject.targetObjects)
        {
            AirFlux airFlux = o as AirFlux;
            airFlux.UpdateParticlesParameters();
        }
    }

    private void UpdateParameters(AirFlux airFlux)
    {
        airFlux.transform.localScale = new Vector3(airFlux.Radius, airFlux.Height, airFlux.Radius);

        ParticleSystem ps = airFlux.transform.Find("AirFluxParticles").GetComponent<ParticleSystem>();

        ps.startLifetime = airFlux.Height *1.7f / 3f;
        ps.maxParticles = (int)(10 * airFlux.Height * airFlux.Radius * airFlux.Radius);
        ps.transform.localScale = new Vector3(1, 1, 1);

        CapsuleCollider collider = airFlux.GetComponent<CapsuleCollider>();
        collider.center = new Vector3(0, 0.5f - airFlux.Radius / airFlux.Height / 2, 0);
        collider.height = 1 + airFlux.Radius / airFlux.Height;
    }
}
