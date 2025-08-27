using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InteractableObject))]
public class InteractableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InteractableObject interactableObject = (InteractableObject)target;

        // Draw regular cursor field
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("regularCursor"));

        // Draw interact cursor field
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("interactCursor"));


        // Draw time for auto end field
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeForAutoEnd"));

        // Check if isCollectible is true
        interactableObject.isCollectible = EditorGUILayout.Toggle("Is Collectible", interactableObject.isCollectible);
        if (interactableObject.isCollectible)
        {
            // Draw theItem field if isCollectible is true
            EditorGUILayout.PropertyField(serializedObject.FindProperty("theItem"));

        }

        // Check if isDialogue is true
        interactableObject.isDialogue = EditorGUILayout.Toggle("Is Dialogue", interactableObject.isDialogue);
        if (interactableObject.isDialogue)
        {
            // Draw isBigDialogue field if isDialogue is true
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isBigDialogue"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dialogueLines"));
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
