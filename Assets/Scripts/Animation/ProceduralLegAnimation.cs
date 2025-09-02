using UnityEngine;

public enum AnimationState { Idle, Walk, Run }

public class ProceduralLegAnimation : MonoBehaviour
{
    [Header("Animation State Control")]
    public AnimationState currentState = AnimationState.Idle;

    [Header("IK Targets")]
    public Transform frontLegTarget;
    public Transform backLegTarget;

    [Header("IK Bone References")]
    [Tooltip("The upper bone of the front leg (the hip or thigh).")]
    public Transform frontLegHip;
    [Tooltip("The lower bone of the front leg (the shin or calf).")]
    public Transform frontLegShin;
    [Tooltip("The upper bone of the back leg.")]
    public Transform backLegHip;
    [Tooltip("The lower bone of the back leg.")]
    public Transform backLegShin;

    [Header("Animation Curve")]
    [Tooltip("Controls the height profile of a step during its swing phase. X-axis is normalized time (0 to 1), Y-axis is the height multiplier.")]
    public AnimationCurve stepHeightCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f, 0f, 0f),
        new Keyframe(0.5f, 1.0f, 0f, 0f),
        new Keyframe(1.0f, 0.0f, 0f, 0f)
    );

    [Header("Shared Parameters")]
    public float stepHeight = 0.4f;
    public float adaptationSpeed = 10.0f;
    [Tooltip("Time in seconds for the IK target to smoothly reach its goal. Lower values are faster/snappier.")]
    public float smoothTime = 0.05f;
    [Tooltip("Prevents knee snapping by keeping the leg slightly bent. 1.0 = fully extended, 0.98 = 98% of max reach.")]
    [Range(0.8f, 1.0f)]
    public float maxExtensionMargin = 0.98f;


    [Tooltip("The actual horizontal speed of the character, controlled by an external script like PlayerManager.")]
    public float currentWorldSpeed = 0f;
    public float multimplierSpeed = 0.5f;
    [Header("Walk Parameters")]
    public float walkStepLength = 0.8f;
    // walkAnimationSpeed is now removed

    [Header("Run Parameters")]
    public float runStepLength = 1.2f;

    private Vector3 _frontLegHomePos;
    private Vector3 _backLegHomePos;
    private float _animationTime;
    private float _currentStepLength;

    private float _frontLegMaxReach;
    private float _backLegMaxReach;

    // For SmoothDamp
    private Vector3 _currentFrontTargetPos;
    private Vector3 _currentBackTargetPos;
    private Vector3 _frontLegVelocity;
    private Vector3 _backLegVelocity;

    void Start()
    {
        _frontLegHomePos = frontLegTarget.localPosition;
        _backLegHomePos = backLegTarget.localPosition;

        // Calculate the maximum possible reach for each leg based on bone lengths
        _frontLegMaxReach = Vector3.Distance(frontLegHip.position, frontLegShin.position) + Vector3.Distance(frontLegShin.position, frontLegTarget.position);
        _backLegMaxReach = Vector3.Distance(backLegHip.position, backLegShin.position) + Vector3.Distance(backLegShin.position, backLegTarget.position);

        _currentFrontTargetPos = frontLegTarget.position;
        _currentBackTargetPos = backLegTarget.position;
    }

    void Update()
    {
        // The logic for calculating animation speed is now inside AnimateLegs,
        // so we just pass the step length directly.
        switch (currentState)
        {
            case AnimationState.Idle:
                ReturnLegsToHome();
                break;
            case AnimationState.Walk:
                AnimateLegs(walkStepLength);
                break;
            case AnimationState.Run:
                AnimateLegs(runStepLength);
                break;
        }
    }

    private void AnimateLegs(float targetStepLength)
    {
        float speed = (multimplierSpeed * Mathf.PI * currentWorldSpeed) / targetStepLength;
        if (targetStepLength <= 0.01f)
        {
            speed = 0f;
        }

        _animationTime += Time.deltaTime * speed;
        _currentStepLength = Mathf.Lerp(_currentStepLength, targetStepLength, Time.deltaTime * adaptationSpeed);

        // --- Calculate Desired Leg Positions ---

        // Forward/backward motion (X-axis) using a sine wave for smooth oscillation
        float frontLegX = -Mathf.Sin(_animationTime) * _currentStepLength / 2f;
        float backLegX = -Mathf.Sin(_animationTime + Mathf.PI) * _currentStepLength / 2f;

        // Upward motion (Y-axis) using the customizable AnimationCurve
        // A leg lifts during its "swing phase" (moving from back to front)
        float frontLegY = CalculateSwingHeight(_animationTime);
        float backLegY = CalculateSwingHeight(_animationTime + Mathf.PI);

        // Combine into a local offset and then transform to world space
        Vector3 desiredFrontPos = transform.TransformPoint(_frontLegHomePos + new Vector3(frontLegX, frontLegY, 0));
        Vector3 desiredBackPos = transform.TransformPoint(_backLegHomePos + new Vector3(backLegX, backLegY, 0));

        // --- Knee Safety Clamp ---
        // Prevent the leg from fully extending to avoid knee snapping
        desiredFrontPos = ClampToMaxReach(desiredFrontPos, frontLegHip.position, _frontLegMaxReach);
        desiredBackPos = ClampToMaxReach(desiredBackPos, backLegHip.position, _backLegMaxReach);

        // --- Smooth IK Movement ---
        // Use SmoothDamp for frame-rate independent, natural-looking smoothing
        _currentFrontTargetPos = Vector3.SmoothDamp(_currentFrontTargetPos, desiredFrontPos, ref _frontLegVelocity, smoothTime);
        _currentBackTargetPos = Vector3.SmoothDamp(_currentBackTargetPos, desiredBackPos, ref _backLegVelocity, smoothTime);

        // Apply the final positions to the IK targets
        frontLegTarget.position = _currentFrontTargetPos;
        backLegTarget.position = _currentBackTargetPos;
    }

    private float CalculateSwingHeight(float time)
    {
        // Determine our position in the full animation cycle (0 to 2*PI)
        float cycleTime = Mathf.Repeat(time, 2f * Mathf.PI);

        // The swing phase (leg in the air) happens in the second half of the cycle,
        // as the leg moves from the rearmost point back to the front.
        // This corresponds to the cycle time being between PI and 2*PI.
        if (cycleTime > Mathf.PI)
        {
            // We are in the swing phase.
            // We'll map the current time in this phase to a 0-1 value
            // to sample the AnimationCurve correctly.
            float swingProgress = (cycleTime - Mathf.PI) / Mathf.PI;
            return stepHeightCurve.Evaluate(swingProgress) * stepHeight;
        }

        // If we're not in the swing phase, the foot is on the ground.
        return 0f;
    }

    private Vector3 ClampToMaxReach(Vector3 desiredPos, Vector3 hipPosition, float maxReach)
    {
        float currentDistance = Vector3.Distance(desiredPos, hipPosition);
        if (currentDistance > maxReach * maxExtensionMargin)
        {
            // If the desired position is too far, we first find the clamped position.
            Vector3 directionFromHip = (desiredPos - hipPosition).normalized;
            Vector3 clampedPos = hipPosition + directionFromHip * (maxReach * maxExtensionMargin);

            // THE FIX: Add a small upward nudge to the clamped position.
            // This ensures the knee always has a slight, consistent bend direction, preventing the snap.
            // You can adjust the 0.05f value if needed, but it should be small.
            Vector3 upwardNudge = transform.up * 0.05f;

            return clampedPos + upwardNudge;
        }
        return desiredPos;
    }


    private void ReturnLegsToHome()
    {
        _currentStepLength = Mathf.Lerp(_currentStepLength, 0, Time.deltaTime * adaptationSpeed);

        Vector3 desiredFrontHome = transform.TransformPoint(_frontLegHomePos);
        Vector3 desiredBackHome = transform.TransformPoint(_backLegHomePos);

        // Use SmoothDamp here as well for a consistent feel
        _currentFrontTargetPos = Vector3.SmoothDamp(_currentFrontTargetPos, desiredFrontHome, ref _frontLegVelocity, smoothTime);
        _currentBackTargetPos = Vector3.SmoothDamp(_currentBackTargetPos, desiredBackHome, ref _backLegVelocity, smoothTime);

        frontLegTarget.position = _currentFrontTargetPos;
        backLegTarget.position = _currentBackTargetPos;

        // Reset animation time when idle to ensure a clean start next time
        if (_currentStepLength < 0.01f)
        {
            _animationTime = 0;
        }
    }
}
