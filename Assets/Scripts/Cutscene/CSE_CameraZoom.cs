using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class CSE_CameraZoom : CutsceneElementBase
{
    private static CSE_CameraZoom instance;
    public static CSE_CameraZoom Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<CSE_CameraZoom>();
            return instance;
        }
    }
    [Header("Camera Settings")]
    [SerializeField] private float targetFOV;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private CinemachineVirtualCamera cutsceneCamera;

    [Header("Letterbox Settings")]
    [SerializeField] private RectTransform topBar; // Top black bar
    [SerializeField] private RectTransform bottomBar; // Bottom black bar
    [SerializeField] private float letterboxHeight = 100f; // Height of the black bars
    [SerializeField] private float letterboxDuration = 1f; // Duration for the letterbox effect

    [Header("Dialogue Settings")]
    [SerializeField] private List<string> dialogueLines; // List to store dialogue lines
    [SerializeField] private bool autoAdvanceDialogue = false; // Whether dialogue auto-advances
    [SerializeField] private float autoAdvanceDelay = 2.0f; // Delay for auto-advance (if enabled)
    [SerializeField] private TextMeshProUGUI customTMP; // Optional TextMeshProUGUI for specific dialogue
    [SerializeField] private CanvasGroup cGroup;

    [Header("Voice Settings")]
    [SerializeField] public bool dialogueHasVoice = true; // Toggle voice options visibility
    [SerializeField] private List<AudioClip> voiceLines; // List of voice clips for each dialogue line
    [SerializeField] private bool waitForAudioFinish = true; // Whether to wait for audio to finish before auto-advancing

    private CinemachineVirtualCamera vCam;

    public override void Execute()
    {
        vCam = cutsceneHandler.vCam;
        vCam.Follow = null;
        cutsceneCamera.Priority = 2;
        PlayerManager.Instance.canMove = false;
        // Start the cutscene effects (Zoom and letterbox)
        StartCoroutine(ZoomCamera());
        StartCoroutine(LetterboxEffect(true)); // Show the letterbox effect
    }

    private IEnumerator ZoomCamera()
    {
        Vector3 originalPosition = vCam.transform.position;
        Vector3 targetPosition = target.position + offset;

        float originalSize = vCam.m_Lens.OrthographicSize;
        float startTime = Time.time;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            vCam.m_Lens.OrthographicSize = Mathf.Lerp(originalSize, targetFOV, t);
            vCam.transform.position = Vector3.Lerp(originalPosition, targetPosition, t);

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        vCam.m_Lens.OrthographicSize = targetFOV;
        vCam.transform.position = targetPosition;

        // Start dialogue after zoom completes
        StartDialogue();
    }

    private void StartDialogue()
    {
        if (dialogueHasVoice)
        {
            SimpleDialogue.Instance.ShowDialogue(dialogueLines, customTMP, voiceLines, cGroup, autoAdvanceDialogue, waitForAudioFinish,true, autoAdvanceDelay);
        }
        else
        {
            SimpleDialogueNoVoice.Instance.ShowDialogue(dialogueLines, customTMP, cGroup, autoAdvanceDialogue , autoAdvanceDelay , true);
        }
    }

    public void hideLetterbox()
    {
        StartCoroutine(LetterboxEffect(false));
    }
    public IEnumerator LetterboxEffect(bool show)
    {
        float startTime = Time.time;
        float elapsedTime = 0;

        // Starting and target heights for the bars (either show or hide them)
        float startHeight = show ? 0 : letterboxHeight;
        float targetHeight = show ? letterboxHeight : 0;

        while (elapsedTime < letterboxDuration)
        {
            float t = elapsedTime / letterboxDuration;
            float currentHeight = Mathf.Lerp(startHeight, targetHeight, t);

            // Update the height of the black bars
            topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, currentHeight);
            bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, currentHeight);

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        // Finalize the bar heights
        topBar.sizeDelta = new Vector2(topBar.sizeDelta.x, targetHeight);
        bottomBar.sizeDelta = new Vector2(bottomBar.sizeDelta.x, targetHeight);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
