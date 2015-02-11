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
}
