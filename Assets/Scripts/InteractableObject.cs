using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [TextArea(3, 10)]
    public List<string> dialogueLines;  // List of dialogue lines

    private Coroutine dialogueCoroutine;

    [SerializeField] public float timeForAutoAdvance; // Time for auto-advance between lines
    [SerializeField] public bool autoAdvanceDialogue; // Whether the dialogue auto-advances
    [SerializeField] public bool isCollectible; // Is this object collectible?
    [SerializeField] public bool isDialogue; // Does this object have dialogue?
    [SerializeField] public InventoryItem theItem; // Item to be collected

    [SerializeField] private List<AudioClip> voiceLines; // List of voice clips for each dialogue line
    [SerializeField] private bool waitForAudioFinish = true; // Whether to wait for audio to finish before auto-advancing
    [SerializeField] private CanvasGroup cGroup;
    private static InteractableObject instance;
    public static InteractableObject Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<InteractableObject>();
            return instance;
        }
    }

    void Start()
    {
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>(); // Ensure the object has a collider
        }
    }

    void Update()
    {
        // Any other update logic can go here
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse entered the object!");
        Cursor.SetCursor(GameManager.Instance.interactCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        Debug.Log("Mouse exited the object!");
        Cursor.SetCursor(GameManager.Instance.regularCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse clicked on the object!");

        if (isCollectible)
        {
            PickupItem(theItem);  // Pick up the item if it's collectible
        }
        else if (isDialogue)
        {
            // Show dialogue with auto-advance or player-controlled mode
            if (dialogueLines != null && dialogueLines.Count > 0)
            {
                if (dialogueCoroutine != null)
                {
                    StopCoroutine(dialogueCoroutine);
                }

                // Check if voiceLines are empty and handle accordingly
                if (voiceLines == null || voiceLines.Count == 0)
                {
                    voiceLines = new List<AudioClip>(new AudioClip[dialogueLines.Count]); // Create empty voice clips array
                }

                // Show dialogue with or without auto-advance
                if (autoAdvanceDialogue)
                {
                    // Pass voice lines to SimpleDialogue for auto-advancing, and whether to wait for audio
                    SimpleDialogue.Instance.ShowDialogue(dialogueLines, null, voiceLines, cGroup, true, waitForAudioFinish,false, timeForAutoAdvance);
                }
                else
                {
                    // No auto-advance, player controls the advancement
                    SimpleDialogue.Instance.ShowDialogue(dialogueLines, null, voiceLines, cGroup, false, waitForAudioFinish);
                }
            }
        }
        else
        {
            // Do nothing if it's not collectible or has dialogue
        }
    }


    public void PickupItem(InventoryItemsBase item)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        inventory.AddItem(item); // Add the item to the inventory
        Cursor.SetCursor(GameManager.Instance.regularCursor, Vector2.zero, CursorMode.Auto); // Reset cursor
        Destroy(gameObject); // Destroy the object after pickup
    }
}
