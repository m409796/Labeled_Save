using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SendTriggerNotif))]
public class SendTriggerNotifEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the script
        SendTriggerNotif script = (SendTriggerNotif)target;

        // Draw everything except _voiceLines and waitForAudioFinish
        DrawPropertiesExcluding(serializedObject, "_voiceLines", "waitForAudioFinish");

        // Now handle _voiceLines and waitForAudioFinish manually
        if (script.dialogueHasVoice)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_voiceLines"), new GUIContent("Voice Lines"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waitForAudioFinish"), new GUIContent("Wait For Audio Finish"));
        }

        // Apply changes to serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
