//// ProceduralWalker.cs - After
//using UnityEngine;

//[RequireComponent(typeof(WalkerAnimator))]
//public class ProceduralWalker : MonoBehaviour
//{
//    // ... (Fields and Awake()/Update() methods from the previous version remain the same)
//    [Header("Input Source")]
//    [Tooltip("The Rigidbody2D used to determine speed.")]
//    public Rigidbody2D rb;
//    [Tooltip("If Rigidbody is not assigned, this speed will be used.")]
//    public float manualSpeed = 0f;

//    [Header("State Control")]
//    [Tooltip("Force the character into a specific move state (useful for preview/testing)")]
//    public bool forceState = false;
//    public MoveState forcedState = MoveState.Idle;

//    [Header("Dependencies")]
//    [Tooltip("Reference to the animator component that handles the visual updates.")]
//    [SerializeField]
//    private WalkerAnimator walkerAnimator;
//    public enum MoveState { Idle, Walk, Run }
//    private MoveState _currentState = MoveState.Idle;

//    void Awake()
//    {
//        if (walkerAnimator == null) walkerAnimator = GetComponent<WalkerAnimator>();
//    }

//    void Update()
//    {
//        if (walkerAnimator == null || walkerAnimator.profile == null) return;

//        float currentSpeed = rb != null ? Mathf.Abs(rb.velocity.x) : Mathf.Abs(manualSpeed);
//        float horizontalInput = rb != null ? rb.velocity.x : manualSpeed;

//        if (forceState)
//        {
//            _currentState = forcedState;
//        }
//        else
//        {
//            if (currentSpeed < 0.1f) _currentState = MoveState.Idle;
//            else if (currentSpeed >= walkerAnimator.profile.runSpeedThreshold) _currentState = MoveState.Run;
//            else _currentState = MoveState.Walk;
//        }

//        walkerAnimator.UpdateAnimation(_currentState, horizontalInput);

//        HandleFlipping(horizontalInput);
//    }

//    // --- NEWLY COMPLETED METHOD ---
//    private void HandleFlipping(float horizontalInput)
//    {
//        // We need to check the profile to see which way the art faces
//        if (walkerAnimator.profile == null) return;

//        if (Mathf.Abs(horizontalInput) > 0.1f)
//        {
//            float moveDirection = Mathf.Sign(horizontalInput);

//            // If the art faces left by default, we need to invert the logic
//            if (!walkerAnimator.profile.artFacesRight)
//            {
//                moveDirection *= -1;
//            }

//            // Apply the flip
//            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
//        }
//    }
//}
