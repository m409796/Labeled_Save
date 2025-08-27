using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SendTriggerNotif : MonoBehaviour
{
    [SerializeField] private List<string> _dialogueLines; // List to store dialogue lines
    [SerializeField] private bool autoAdvanceDialogue = false; // Whether dialogue auto-advances
    [SerializeField] private float timeForAutoAdvance = 2.0f; // Delay for auto-advance (if enabled)
    [SerializeField] private TextMeshProUGUI customTMP; // Optional TextMeshProUGUI for specific dialogue
    [SerializeField] public bool dialogueHasVoice = true;
    [SerializeField] private List<AudioClip> _voiceLines; // List of voice clips for each dialogue line
    [SerializeField] private bool waitForAudioFinish = true; // Whether to wait for audio to finish before auto-advancing
    [SerializeField] private CanvasGroup cGroup;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (dialogueHasVoice)
            {
                SimpleDialogue.Instance.ShowDialogue(_dialogueLines, customTMP, _voiceLines, cGroup, autoAdvanceDialogue, waitForAudioFinish, false, timeForAutoAdvance);

            }
            else
            {
                SimpleDialogueNoVoice.Instance.ShowDialogue(_dialogueLines, customTMP, cGroup, autoAdvanceDialogue, timeForAutoAdvance, false);
            }
            // Call the ShowDialogue method from SimpleDialogue
        }
    }
}
