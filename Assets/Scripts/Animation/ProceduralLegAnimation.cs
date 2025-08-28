using UnityEngine;

public enum AnimationState { Idle, Walk, Run }

public class ProceduralLegAnimation : MonoBehaviour
{
    [Header("Animation State Control")]
    public AnimationState currentState = AnimationState.Idle;

    [Header("IK Targets")]
    public Transform frontLegTarget;
    public Transform backLegTarget;

    [Header("IK Bone References (For Knee Safety)")]
    [Tooltip("The upper bone of the front leg (the hip or thigh).")]
    public Transform frontLegHip;
    [Tooltip("The lower bone of the front leg (the shin or calf).")]
    public Transform frontLegShin;
    [Tooltip("The upper bone of the back leg.")]
    public Transform backLegHip;
    [Tooltip("The lower bone of the back leg.")]
    public Transform backLegShin;

    [Header("Shared Parameters")]
    public float stepHeight = 0.4f;
    public float adaptationSpeed = 10.0f;
    [Tooltip("How smoothly the IK targets move. Prevents jitter when legs are extended. Higher = more responsive, Lower = smoother.")]
    public float ikSmoothingSpeed = 20.0f;

    [Header("Walk Parameters")]
    public float walkStepLength = 0.8f;
    public float walkAnimationSpeed = 5.0f;

    [Header("Run Parameters")]
    public float runStepLength = 1.2f;
    public float runAnimationSpeed = 8.0f;

    private Vector3 _frontLegHomePos;
    private Vector3 _backLegHomePos;
    private float _animationTime;
    private float _currentStepLength;

    private float _frontLegMaxReach;
    private float _backLegMaxReach;

    // These will store the smoothed-out positions for the IK targets
    private Vector3 _currentFrontTargetPos;
    private Vector3 _currentBackTargetPos;

    void Start()
    {
        _frontLegHomePos = frontLegTarget.localPosition;
        _backLegHomePos = backLegTarget.localPosition;

        _frontLegMaxReach = Vector3.Distance(frontLegHip.position, frontLegShin.position) + Vector3.Distance(frontLegShin.position, frontLegTarget.position);
        _backLegMaxReach = Vector3.Distance(backLegHip.position, backLegShin.position) + Vector3.Distance(backLegShin.position, backLegTarget.position);

        // Initialize the smoothed positions to the actual starting positions
        _currentFrontTargetPos = frontLegTarget.position;
        _currentBackTargetPos = backLegTarget.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case AnimationState.Idle:
                ReturnLegsToHome();
                break;
            case AnimationState.Walk:
                AnimateLegs(walkStepLength, walkAnimationSpeed);
                break;
            case AnimationState.Run:
                AnimateLegs(runStepLength, runAnimationSpeed);
                break;
        }
    }

    private void AnimateLegs(float targetStepLength, float speed)
    {
        _animationTime += Time.deltaTime * speed;
        _currentStepLength = Mathf.Lerp(_currentStepLength, targetStepLength, Time.deltaTime * adaptationSpeed);

        float frontLegX = Mathf.Sin(_animationTime) * _currentStepLength / 2;
        float frontLegY = Mathf.Max(0f, Mathf.Cos(_animationTime)) * stepHeight;

        float backLegX = Mathf.Sin(_animationTime + Mathf.PI) * _currentStepLength / 2;
        float backLegY = Mathf.Max(0f, Mathf.Cos(_animationTime + Mathf.PI)) * stepHeight;

        Vector3 desiredFrontPos = transform.TransformPoint(_frontLegHomePos + new Vector3(frontLegX, frontLegY, 0));
        Vector3 desiredBackPos = transform.TransformPoint(_backLegHomePos + new Vector3(backLegX, backLegY, 0));

        // --- KNEE-SAVING CLAMP ---
        // As an extra precaution, you can make the safety margin a little bigger here,
        // from 0.99f to 0.98f, to further reduce the chance of full extension.
        float frontDist = Vector3.Distance(desiredFrontPos, frontLegHip.position);
        if (frontDist > _frontLegMaxReach)
        {
            desiredFrontPos = frontLegHip.position + (desiredFrontPos - frontLegHip.position).normalized * (_frontLegMaxReach * 0.99f);
        }

        float backDist = Vector3.Distance(desiredBackPos, backLegHip.position);
        if (backDist > _backLegMaxReach)
        {
            desiredBackPos = backLegHip.position + (desiredBackPos - backLegHip.position).normalized * (_backLegMaxReach * 0.99f);
        }

        // --- IK SMOOTHING ---
        // Smoothly interpolate our current target position towards the desired one.
        _currentFrontTargetPos = Vector3.Lerp(_currentFrontTargetPos, desiredFrontPos, Time.deltaTime * ikSmoothingSpeed);
        _currentBackTargetPos = Vector3.Lerp(_currentBackTargetPos, desiredBackPos, Time.deltaTime * ikSmoothingSpeed);

        // Apply the final, smoothed, and clamped positions
        frontLegTarget.position = _currentFrontTargetPos;
        backLegTarget.position = _currentBackTargetPos;
    }

    private void ReturnLegsToHome()
    {
        _currentStepLength = Mathf.Lerp(_currentStepLength, 0, Time.deltaTime * adaptationSpeed);

        // Also apply smoothing when returning home for consistency
        Vector3 desiredFrontHome = transform.TransformPoint(_frontLegHomePos);
        Vector3 desiredBackHome = transform.TransformPoint(_backLegHomePos);

        _currentFrontTargetPos = Vector3.Lerp(_currentFrontTargetPos, desiredFrontHome, Time.deltaTime * ikSmoothingSpeed);
        _currentBackTargetPos = Vector3.Lerp(_currentBackTargetPos, desiredBackHome, Time.deltaTime * ikSmoothingSpeed);

        frontLegTarget.position = _currentFrontTargetPos;
        backLegTarget.position = _currentBackTargetPos;
    }
}