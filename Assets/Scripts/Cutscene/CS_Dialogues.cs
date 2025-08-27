using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CS_Dialogues : SendTriggerNotif
{
    private static CS_Dialogues instance;
    public static CS_Dialogues Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<CS_Dialogues>();
            return instance;
        }
    }
    [SerializeField] private List<string> _dialogueLines; // List to store dialogue lines
    [SerializeField] private bool autoAdvanceDialogue = false; // Whether dialogue auto-advances
    [SerializeField] private float timeForAutoAdvance = 2.0f; // Delay for auto-advance (if enabled)
    [SerializeField] private TextMeshProUGUI customTMP; // Optional TextMeshProUGUI for specific dialogue
    [SerializeField] private List<AudioClip> _voiceLines; // List of voice clips for each dialogue line
    [SerializeField] private bool waitForAudioFinish = true; // Whether to wait for audio to finish before auto-advancing



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //public void TriggerDialogue()
    //{
    //    SimpleDialogue.Instance.ShowDialogue(_dialogueLines, customTMP, _voiceLines, autoAdvanceDialogue, waitForAudioFinish, timeForAutoAdvance);
    //}
}
