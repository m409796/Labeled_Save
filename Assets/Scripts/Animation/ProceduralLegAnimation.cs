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

    [Header("Natural Walking Curves")]
    [Tooltip("Controls the height profile during swing phase. More natural arch.")]
    public AnimationCurve stepHeightCurve = new AnimationCurve(
        new Keyframe(0.0f, 0.0f, 0f, 2f),    // Start on ground, slight lift
        new Keyframe(0.3f, 0.8f, 0f, 0f),    // Peak earlier for natural step
        new Keyframe(0.7f, 0.6f, 0f, 0f),    // Slight dip (foot preparing to land)
        new Keyframe(1.0f, 0.0f, -2f, 0f)    // Land with slight forward motion
    );

    [Tooltip("Controls horizontal movement during swing. Creates natural stride.")]
    public AnimationCurve stepStrideCurve = new AnimationCurve(
        new Keyframe(0.0f, -1.0f, 0f, 0f),   // Start at back of stride
        new Keyframe(0.2f, -0.7f, 2f, 2f),   // Quick acceleration forward
        new Keyframe(0.8f, 0.7f, 2f, 2f),    // Continue forward motion
        new Keyframe(1.0f, 1.0f, 0f, 0f)     // End at front of stride
    );

    [Tooltip("Controls stance phase horizontal movement (foot on ground).")]
    public AnimationCurve stanceMoveCurve = new AnimationCurve(
        new Keyframe(0.0f, 1.0f, 0f, 0f),    // Foot plants forward
        new Keyframe(0.5f, 0.0f, -2f, -2f),  // Moves to center
        new Keyframe(1.0f, -1.0f, 0f, 0f)    // Pushes off from back
    );

    [Header("Human-like Parameters")]
    public float stepHeight = 0.4f;
    public float adaptationSpeed = 10.0f;
    [Tooltip("Time in seconds for smooth IK transitions.")]
    public float smoothTime = 0.08f;
    [Range(0.8f, 1.0f)]
    public float maxExtensionMargin = 0.98f;

    [Header("Walking Dynamics")]
    [Tooltip("Stance phase takes this percentage of the full cycle (60% is natural).")]
    [Range(0.4f, 0.8f)]
    public float stancePhaseRatio = 0.6f;

    [Tooltip("Adds slight randomness to step timing for natural variation.")]
    [Range(0f, 0.1f)]
    public float stepTimingVariation = 0.03f;

    [Tooltip("Forward lean amount during walking (creates momentum feel).")]
    [Range(0f, 0.2f)]
    public float forwardLeanOffset = 0.05f;

    [Tooltip("Weight shift during stance phase.")]
    [Range(0f, 0.1f)]
    public float weightShiftAmount = 0.03f;

    [Tooltip("The actual horizontal speed of the character, controlled by an external script like PlayerManager.")]
    public float currentWorldSpeed = 0f;
    public float multimplierSpeed = 0.5f;

    [Header("Walk Parameters")]
    public float walkStepLength = 0.8f;

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

    // Natural walking variables
    private float _frontLegPhaseOffset;
    private float _backLegPhaseOffset;
    private float _nextVariationTime;

    void Start()
    {
        _frontLegHomePos = frontLegTarget.localPosition;
        _backLegHomePos = backLegTarget.localPosition;

        // Calculate the maximum possible reach for each leg based on bone lengths
        _frontLegMaxReach = Vector3.Distance(frontLegHip.position, frontLegShin.position) + Vector3.Distance(frontLegShin.position, frontLegTarget.position);
        _backLegMaxReach = Vector3.Distance(backLegHip.position, backLegShin.position) + Vector3.Distance(backLegShin.position, backLegTarget.position);

        _currentFrontTargetPos = frontLegTarget.position;
        _currentBackTargetPos = backLegTarget.position;

        // Initialize phase offsets for natural variation
        _frontLegPhaseOffset = 0f;
        _backLegPhaseOffset = Mathf.PI; // Back leg starts opposite
        _nextVariationTime = Time.time + 2f;
    }

    void Update()
    {
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

        // Add subtle timing variations for natural feel
        if (Time.time > _nextVariationTime && stepTimingVariation > 0)
        {
            _frontLegPhaseOffset += Random.Range(-stepTimingVariation, stepTimingVariation);
            _backLegPhaseOffset += Random.Range(-stepTimingVariation, stepTimingVariation);
            _nextVariationTime = Time.time + Random.Range(1.5f, 3f);
        }

        // Calculate individual leg phases
        float frontLegPhase = _animationTime + _frontLegPhaseOffset;
        float backLegPhase = _animationTime + _backLegPhaseOffset;

        // --- Calculate Natural Leg Positions ---
        Vector3 frontLegOffset = CalculateNaturalLegMotion(frontLegPhase, true);
        Vector3 backLegOffset = CalculateNaturalLegMotion(backLegPhase, false);

        // Apply forward lean for momentum feel
        frontLegOffset.x += forwardLeanOffset;
        backLegOffset.x += forwardLeanOffset;

        // Transform to world space
        Vector3 desiredFrontPos = transform.TransformPoint(_frontLegHomePos + frontLegOffset);
        Vector3 desiredBackPos = transform.TransformPoint(_backLegHomePos + backLegOffset);

        // --- Knee Safety Clamp ---
        desiredFrontPos = ClampToMaxReach(desiredFrontPos, frontLegHip.position, _frontLegMaxReach);
        desiredBackPos = ClampToMaxReach(desiredBackPos, backLegHip.position, _backLegMaxReach);

        // --- Smooth IK Movement ---
        _currentFrontTargetPos = Vector3.SmoothDamp(_currentFrontTargetPos, desiredFrontPos, ref _frontLegVelocity, smoothTime);
        _currentBackTargetPos = Vector3.SmoothDamp(_currentBackTargetPos, desiredBackPos, ref _backLegVelocity, smoothTime);

        frontLegTarget.position = _currentFrontTargetPos;
        backLegTarget.position = _currentBackTargetPos;
    }

    private Vector3 CalculateNaturalLegMotion(float legPhase, bool isFrontLeg)
    {
        // Normalize phase to 0-2π cycle
        float normalizedPhase = Mathf.Repeat(legPhase, 2f * Mathf.PI);
        float cycleProgress = normalizedPhase / (2f * Mathf.PI);

        // Determine if leg is in stance (on ground) or swing (in air) phase
        bool isInSwingPhase = cycleProgress > stancePhaseRatio;

        float legX, legY;

        if (isInSwingPhase)
        {
            // SWING PHASE: Leg is in the air, moving forward to next step
            float swingProgress = (cycleProgress - stancePhaseRatio) / (1f - stancePhaseRatio);

            // CORRECTED: During swing, leg moves from back to front (negative curve value)
            legX = -stepStrideCurve.Evaluate(swingProgress) * _currentStepLength / 2f;

            // Use height curve for vertical movement
            legY = stepHeightCurve.Evaluate(swingProgress) * stepHeight;
        }
        else
        {
            // STANCE PHASE: Leg is on ground, body moves over it
            float stanceProgress = cycleProgress / stancePhaseRatio;

            // CORRECTED: During stance, foot slides from front to back as body moves forward
            legX = -stanceMoveCurve.Evaluate(stanceProgress) * _currentStepLength / 2f;

            // Slight weight shift during stance
            legY = -weightShiftAmount * Mathf.Sin(stanceProgress * Mathf.PI);
        }

        // Add subtle asymmetry between front and back legs
        if (!isFrontLeg)
        {
            legX *= 0.95f; // Back leg slightly shorter stride
            legY *= 1.05f; // Back leg slightly higher lift
        }

        return new Vector3(legX, legY, 0);
    }

    private Vector3 ClampToMaxReach(Vector3 desiredPos, Vector3 hipPosition, float maxReach)
    {
        float currentDistance = Vector3.Distance(desiredPos, hipPosition);
        if (currentDistance > maxReach * maxExtensionMargin)
        {
            Vector3 directionFromHip = (desiredPos - hipPosition).normalized;
            Vector3 clampedPos = hipPosition + directionFromHip * (maxReach * maxExtensionMargin);

            // THE FIX: Add a small upward nudge to the clamped position.
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
            // Reset phase offsets when returning to idle
            _frontLegPhaseOffset = 0f;
            _backLegPhaseOffset = Mathf.PI;
        }
    }
}
