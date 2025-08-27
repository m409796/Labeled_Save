using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDialogueNoVoice : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    private List<string> _dialogueLines; // List of dialogue lines
    private int _lineIndex;

    [SerializeField] private TextMeshProUGUI text; // TextMeshProUGUI for displaying dialogue

    private bool _started;
    private bool _isAutoAdvance; // Whether auto-advancement is enabled
    private bool _isCutScene;
    private CanvasGroup dialogueCGroup;
    private static SimpleDialogueNoVoice instance;
    public static SimpleDialogueNoVoice Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SimpleDialogueNoVoice>();
                if (instance == null)
                {
                    Debug.LogError("No instance of SimpleDialogueNoVoice found in the scene.");
                }
            }
            return instance;
        }
    }

    [SerializeField]
    private List<float> _delays; // List of delay times for each dialogue line
    public float _defaultDelay = 1f; // Default delay time for lines that don't have a specific delay

    public void ShowDialogue(List<string> dialogueLines, TextMeshProUGUI customTMP, CanvasGroup group, bool isAutoAdvance, float delayed , bool isCutscene)
    {
        _dialogueLines = dialogueLines;
        _lineIndex = 0;
        _started = true;
        _isAutoAdvance = isAutoAdvance;
        _isCutScene = isCutscene;
        // Set the default delay to the value passed in or use 1f if not provided
        _defaultDelay = delayed;
        if (group != null)
        {
            dialogueCGroup = group;
        }
        text = customTMP != null ? customTMP : text;
        group.alpha = 1f;
        // Start the typing effect for the first dialogue line
        StartCoroutine(TypeDialogueLine(_dialogueLines[_lineIndex]));
    }

    private IEnumerator TypeDialogueLine(string line)
    {
        // Loop through each character in the line
        for (int i = 0; i < line.Length; i++)
        {
            text.text = line.Substring(0, i + 1); // Update the text to show the current character
            yield return new WaitForSeconds(0.05f); // Add a short delay between characters (adjust as needed)
        }

        // Wait for the specified delay before advancing to the next line
        float delay = (_lineIndex < _delays.Count) ? _delays[_lineIndex] : _defaultDelay;
        yield return new WaitForSeconds(delay);

        AdvanceDialogue();
    }

    private IEnumerator WaitForTimeForEachLine()
    {
        // Loop through all dialogue lines
        for (int i = 0; i < _dialogueLines.Count; i++)
        {
            // Get delay for the current line, if not specified, use the default delay
            float delay = (i < _delays.Count) ? _delays[i] : _defaultDelay;

            // Wait for the specified delay before advancing to the next line
            yield return new WaitForSeconds(delay);

            // Advance dialogue
            AdvanceDialogue();
        }

        EndDialogue(); // End dialogue after all lines are displayed
    }

    public void AdvanceDialogue()
    {
        _lineIndex++;

        if (_lineIndex < _dialogueLines.Count)
        {
            // Start typing the next line of dialogue
            StartCoroutine(TypeDialogueLine(_dialogueLines[_lineIndex]));
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        _started = false;
        text.text = ""; // Clear the dialogue text
        Debug.Log("Dialogue ended.");
        dialogueCGroup.alpha = 0f;
        if (_isCutScene)
        {
            CutsceneHandler.Instance.ManualEndCutscene();
        }
    }
}
