using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CSE_CameraZoom))]
public class CSE_CameraZoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the script
        CSE_CameraZoom script = (CSE_CameraZoom)target;

        // Draw everything except the voice-related fields
        DrawPropertiesExcluding(serializedObject, "voiceLines", "waitForAudioFinish");

        // Now handle voiceLines and waitForAudioFinish manually
        if (script.dialogueHasVoice)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("voiceLines"), new GUIContent("Voice Lines"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForAudioFinish"), new GUIContent("Wait For Audio Finish"));
        }

        // Apply changes to serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
