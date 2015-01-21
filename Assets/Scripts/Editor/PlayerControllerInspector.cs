using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerInspector : Editor
{
    
    PlayerController playerController;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        playerController = target as PlayerController;

        GUILayout.BeginVertical("box");
        GUILayout.Label("Terminal bottom speed: " + playerController.getTerminalBottomSpeed());
        GUILayout.Label("Terminal front speed: " + playerController.getTerminalFrontSpeed());
        GUILayout.EndVertical();

    }
}
