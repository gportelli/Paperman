using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerAerodynamicsController))]
public class PlayerAerodynamicsInspector : Editor
{
    PlayerAerodynamicsController aerodynamics;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        aerodynamics = target as PlayerAerodynamicsController;

        EditorGUILayout.HelpBox(
            string.Format("Terminal bottom speed: {0,2:f} Km/h\nTerminal front speed: {1,2:f} Km/h\nTerminal right speed: {2,2:f} Km/h",
                aerodynamics.getTerminalBottomSpeed() * 3.6,
                aerodynamics.getTerminalFrontSpeed() * 3.6,
                 aerodynamics.getTerminalrightSpeed() * 3.6)
            , MessageType.Info);
    }
}
