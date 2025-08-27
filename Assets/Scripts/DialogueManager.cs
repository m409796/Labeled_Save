//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class DialogueManager : MonoBehaviour
//{
//    public TextMeshProUGUI dialogueText;
//    public GameObject dialoguePanel;
//    public Transform character;


//    private static DialogueManager instance;
//    public static DialogueManager Instance
//    {
//        get
//        {
//            if (instance == null) instance = GameObject.FindObjectOfType<DialogueManager>();
//            return instance;
//        }
//    }
//    private void Start()
//    {
//        dialoguePanel.SetActive(false);
//    }

//    public void ShowDialogue(string message)
//    {
//        dialogueText.text = message;
//        dialoguePanel.SetActive(true);
//        PositionDialogueBox();
//        AdjustDialoguePanelSize();
//    }

//    public void HideDialogue()
//    {
//        dialoguePanel.SetActive(false);
//    }

//    private void PositionDialogueBox()
//    {
//        Vector3 characterScreenPos = Camera.main.WorldToScreenPoint(character.position);
//        dialoguePanel.transform.position = characterScreenPos + new Vector3(0, 50, 0);
//    }

//    private void AdjustDialoguePanelSize()
//    {
//        LayoutRebuilder.ForceRebuildLayoutImmediate(dialoguePanel.GetComponent<RectTransform>());
//    }
//}