using UnityEngine;

public class Door : MonoBehaviour
{
    public int doorNumber; // Assign a unique number to each door in the Inspector
    private DoorManager doorManager;
    private bool playerInRange = false;
    [SerializeField] private GameObject doorSign;
    [SerializeField] private Transform doorPosition;

    void Start()
    {
        doorManager = FindObjectOfType<DoorManager>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            doorManager.OnDoorOpened(doorNumber, doorPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            doorSign.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            doorSign.SetActive(false);
        }
    }
}
