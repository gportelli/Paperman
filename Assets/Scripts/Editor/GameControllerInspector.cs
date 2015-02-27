using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GameController))]
public class GameControllerInspector : Editor {
    private bool showLevels;

    public override void OnInspectorGUI()
    {
        GameController gc = target as GameController;

        //DrawDefaultInspector(); return;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("DebugOn"));
        if (gc.DebugOn)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SkipIntro"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartFromCurrent"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StartLevel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("MusicOn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InvertAxis"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InfiniteBoost"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("KinematicMode"));
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Menu"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Levels"), true);
        
        serializedObject.ApplyModifiedProperties();
    }
}
