using UnityEngine;
using UnityEngine.SceneManagement; // If you want to reload the scene
using UnityEngine.Events;

public class DoorManager : MonoBehaviour
{
    public int[] correctOrder; // Set this in the Inspector (e.g., 1, 2, 3)
    private int currentStep = 0;

    private GameObject player;
    [SerializeField] private GameObject doorGuide;
    public UnityEvent DoorPuzzleCompleted;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Make sure player has "Player" tag
        doorGuide.SetActive(true);
    }

    public void OnDoorOpened(int doorNumber, Transform doorPosition)
    {
        if (correctOrder[currentStep] == doorNumber)
        {
            doorGuide.SetActive(false);
            currentStep++;
        }
        else
        {
            Debug.Log("Wrong Door! Restarting...");
            doorGuide.SetActive(true);
            currentStep = 0; // Reset puzzle progress
        }

        // Move player to the door's position after every interaction (correct or not)
        MovePlayerToDoor(doorPosition);

        if (currentStep >= correctOrder.Length)
        {
            doorGuide.SetActive(false);
            DoorPuzzleCompleted?.Invoke();
            Debug.Log("Puzzle Completed!");
            // Load next area or progress in the game
        }
    }

    void MovePlayerToDoor(Transform doorPosition)
    {
        if (player != null)
        {
            player.transform.position = doorPosition.position; // Move player to the door position
        }
    }
}
