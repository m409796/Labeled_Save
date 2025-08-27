using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDialogue : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    private List<string> _dialogueLines; // List of dialogue lines
    private int _lineIndex;

    [SerializeField] private TextMeshProUGUI text; // TextMeshProUGUI for displaying dialogue
    [SerializeField] private AudioSource audioSource; // AudioSource to play voice lines

    private List<AudioClip> _voiceLines; // List of voice clips corresponding to each dialogue line
    private bool _started;
    private bool _isAutoAdvance; // Whether auto-advancement is enabled
    private bool _waitForAudioFinish; // Flag to wait for audio to finish before advancing

    public float typingSpeed = 0.05f; // Time interval between characters
    private Coroutine typingCoroutine; // Reference to the typing coroutine
    private CanvasGroup dialogueCGroup; 
    private bool _isCutScene;

    private static SimpleDialogue instance;
    public static SimpleDialogue Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SimpleDialogue>();
                if (instance == null)
                {
                    Debug.LogError("No instance of SimpleDialogue found in the scene.");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>(); // Ensure AudioSource is attached
        }
    }

    public void ShowDialogue(List<string> dialogueLines, TextMeshProUGUI customTMP, List<AudioClip> voiceLines, CanvasGroup group, bool isAutoAdvance, bool waitForAudioFinish, bool isCutscene = false, float timeForAutoAdvance = 0f  )
    {
        _dialogueLines = dialogueLines;
        _lineIndex = 0;
        _voiceLines = voiceLines;
        _started = true;
        _isAutoAdvance = isAutoAdvance;
        _waitForAudioFinish = waitForAudioFinish; // Set whether to wait for audio to finish before advancing

        _isCutScene = isCutscene;
        if(group != null)
        {
            dialogueCGroup = group;
        }
        text = customTMP != null ? customTMP : text;

        group.alpha = 1f;
        // Start typing the first dialogue line
        StartTyping(_dialogueLines[_lineIndex]);

        PlayVoiceLine(_lineIndex);

        // Auto-advance logic
        if (isAutoAdvance)
        {
            if (_waitForAudioFinish)
            {
                StartCoroutine(WaitForAudioAndAutoAdvance());
            }
            else
            {
                StartCoroutine(WaitForTimeAndAutoAdvance(timeForAutoAdvance));
            }
        }
    }

    private void StartTyping(string line)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        text.text = ""; // Clear the text
        foreach (char c in line)
        {
            text.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        typingCoroutine = null; // Clear the coroutine reference once typing is complete
    }

    private void PlayVoiceLine(int index)
    {
        if (_voiceLines != null && _voiceLines.Count > index && _voiceLines[index] != null)
        {
            audioSource.clip = _voiceLines[index];
            audioSource.Play();
        }
    }

    private IEnumerator WaitForAudioAndAutoAdvance()
    {
        if (audioSource.clip != null)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {
            // Add a fallback delay if no audio clip is available
            yield return new WaitForSeconds(typingSpeed * _dialogueLines[_lineIndex].Length);
        }

        AdvanceDialogue();
    }


    private IEnumerator WaitForTimeAndAutoAdvance(float timeForAutoAdvance)
    {
        if (audioSource.clip != null)
        {
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
        {
            // Instead of relying on typing speed, set a fixed fallback duration
            yield return new WaitForSeconds(timeForAutoAdvance); // Adjust this time as needed
        }

        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (typingCoroutine != null)
        {
            // If still typing, complete the current line instantly
            StopCoroutine(typingCoroutine);
            text.text = _dialogueLines[_lineIndex];
            typingCoroutine = null;
            return;
        }

        _lineIndex++;
        if (_lineIndex < _dialogueLines.Count)
        {
            StartTyping(_dialogueLines[_lineIndex]);
            PlayVoiceLine(_lineIndex);

            if (_isAutoAdvance && _waitForAudioFinish)
            {
                StartCoroutine(WaitForAudioAndAutoAdvance());
            }
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
