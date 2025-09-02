using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // --- Singleton Instance ---
    private static PlayerManager instance;
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PlayerManager>();
            return instance;
        }
    }

    // --- Component References ---
    [Header("Component References")]
    public Rigidbody2D theRB;
    public ProceduralLegAnimation legAnimator;
    public ProceduralWalker upperBodyAnimator;

    // --- Movement Parameters ---
    [Header("Movement Speeds")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public bool canMove = true;

    // --- Character State ---
    private bool isFacingRight = false;

    // --- Unity Methods ---
    void Start()
    {
        // Automatically find components if they haven't been assigned in the Inspector.
        if (theRB == null)
        {
            theRB = GetComponent<Rigidbody2D>();
            Debug.LogWarning("PlayerManager: Rigidbody2D was not assigned, attempting to find it on the same GameObject.");
        }
        if (legAnimator == null)
        {
            legAnimator = FindObjectOfType<ProceduralLegAnimation>();
            Debug.LogWarning("PlayerManager: ProceduralLegAnimation was not assigned, attempting to find it in the scene.");
        }
        if (upperBodyAnimator == null)
        {
            upperBodyAnimator = FindObjectOfType<ProceduralWalker>();
            Debug.LogWarning("PlayerManager: ProceduralWalker was not assigned, attempting to find it in the scene.");
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (canMove)
        {
            // 1. Handle Movement
            float currentMoveSpeed = isRunning ? runSpeed : walkSpeed;
            theRB.velocity = new Vector2(horizontalInput * currentMoveSpeed, theRB.velocity.y);

            // 2. <<-- NEW -->> Sync Animation Speed with Rigidbody Velocity
            // This is the key change: we read the actual horizontal speed from the Rigidbody
            // and pass it to the leg animator to prevent foot sliding.
            legAnimator.currentWorldSpeed = Mathf.Abs(theRB.velocity.x);

            // 3. Handle Flipping
            if (horizontalInput > 0 && !isFacingRight)
            {
                Flip(); // Moving right but facing left, so flip.
            }
            else if (horizontalInput < 0 && isFacingRight)
            {
                Flip(); // Moving left but facing right, so flip.
            }

            // 4. Handle Animation State for BOTH Leg and Body Animators
            upperBodyAnimator.forceState = true;

            if (Mathf.Abs(horizontalInput) > 0.01f) // If the character is moving
            {
                legAnimator.currentState = isRunning ? AnimationState.Run : AnimationState.Walk;
                upperBodyAnimator.forcedState = isRunning ? ProceduralWalker.MoveState.Run : ProceduralWalker.MoveState.Walk;
            }
            else
            {
                legAnimator.currentState = AnimationState.Idle;
                upperBodyAnimator.forcedState = ProceduralWalker.MoveState.Idle;
            }
        }
        else
        {
            theRB.velocity = new Vector2(0, theRB.velocity.y);
            legAnimator.currentState = AnimationState.Idle;

            // <<-- NEW -->> Ensure animation speed is zeroed out when movement is disabled.
            legAnimator.currentWorldSpeed = 0f;

            upperBodyAnimator.forceState = true;
            upperBodyAnimator.forcedState = ProceduralWalker.MoveState.Idle;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
