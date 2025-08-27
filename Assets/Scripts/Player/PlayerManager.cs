using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlayerManager>();
            return instance;
        }
    }

    public Rigidbody2D theRB;

    [Header("Movement Speeds")]
    public float walkSpeed = 4f; // Renamed for clarity and given a new default
    public float runSpeed = 7f;  // NEW: The speed when holding the run key

    public bool canMove = true;

    void Start()
    {
        if (theRB == null)
        {
            theRB = GetComponent<Rigidbody2D>();
            Debug.LogWarning("PlayerManager: Rigidbody2D was not assigned, attempting to find it on the same GameObject.");
        }
    }

    void Update()
    {
        if (canMove)
        {
            // Determine which speed to use
            float currentMoveSpeed = walkSpeed; // Default to walk speed
            if (Input.GetKey(KeyCode.LeftShift)) // Check if the Left Shift key is held down
            {
                currentMoveSpeed = runSpeed; // If so, use the run speed
            }

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            theRB.velocity = new Vector2(horizontalInput * currentMoveSpeed, theRB.velocity.y);
        }
        else
        {
            theRB.velocity = new Vector2(0, theRB.velocity.y);
        }
    }
}
