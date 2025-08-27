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
    public float moveSpeed = 5f;
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
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            theRB.velocity = new Vector2(horizontalInput * moveSpeed, theRB.velocity.y);
        }
        else
        {
            theRB.velocity = new Vector2(0, theRB.velocity.y);
        }
    }

    // NOTE: The Flip() function and all Animator/SpriteRenderer logic has been removed.
    // The ProceduralWalker.cs script now handles all visual flipping.
}