// Filename: ProceduralWalker.cs
using UnityEngine;

public class ProceduralWalker : MonoBehaviour
{
    [Header("Art and Direction")]
    [Tooltip("Set this to TRUE if your original sprite art faces to the right.")]
    public bool artFacesRight = true;

    [Header("REQUIRED Body Parts")]
    public Transform body;
    public Transform neck;
    public Transform head;
    public Transform frontArm;
    public Transform backArm;
    public Transform frontLeg;
    public Transform backLeg;

    [Header("OPTIONAL Body Parts")]
    public Transform frontLowerArm;
    public Transform backLowerArm;
    public Transform frontLowerLeg;
    public Transform backLowerLeg;
    public Transform frontFoot;
    public Transform backFoot;
    public Transform eyes;
    public Transform tailBase;
    public Transform tailMid;
    public Transform tailTip;

    [Header("Animation Amplitudes (Base defaults - used to initialize profiles)")]
    public float walkCycleSpeed = 5.5f;
    public float bodyBobAmount = 0.04f;
    public float armSwingAngle = 20f;
    public float legSwingAngle = 25f;
    public float kneeBendAngle = 45f;
    public float elbowDragAngle = -20f;
    public float legLiftAmount = 0.05f;
    public float headBobAmount = 0.04f;
    public float neckSwingAngle = 10f;
    public float footAngleMultiplier = 0.5f;
    public float tailSwingAngle = 25f;
    public float tailSegmentDelay = 0.15f; // A small delay in phase for the tail wave

    // --- Run parameters (legacy multipliers kept for compatibility, but per-state profiles are preferred) ---
    [Header("Run Multipliers (Legacy)")]
    [Tooltip("Horizontal speed (units/sec) above which character is considered running.")]
    public float runSpeedThreshold = 5f;
    public float runCycleSpeedMultiplier = 1.6f;
    public float runLegSwingMultiplier = 1.5f;
    public float runBodyBobMultiplier = 1.15f;
    public float runLegLiftMultiplier = 1.2f;

    // New: per-state profiles for fine control in the Inspector
    [System.Serializable]
    public class MotionProfile
    {
        [Header("Timing / Magnitudes")]
        public float cycleSpeed = 5.5f;
        public float legSwingAngle = 25f;
        public float bodyBobAmount = 0.04f;
        public float legLiftAmount = 0.05f;
        public float armSwingAngle = 20f;
        public float kneeBendAngle = 45f;
        public float elbowDragAngle = -20f;
        public float headBobAmount = 0.04f;
        public float neckSwingAngle = 10f;
        public float footAngleMultiplier = 0.5f;
        public float tailSwingAngle = 25f;
    }

    [Header("State Profiles (Idle / Walk / Run)")]
    public MotionProfile idleProfile;
    public MotionProfile walkProfile;
    public MotionProfile runProfile;

    [Header("State Control")]
    [Tooltip("If true, uses the profiles above for per-state parameters")]
    public bool useStateProfiles = true;
    [Tooltip("Force the character into a specific move state (useful for preview/testing)")]
    public bool forceState = false;
    public MoveState forcedState = MoveState.Idle;


    // --- Idle / Blinking ---
    [Header("Idle / Blinking")]
    public float idleBreathAmount = 0.02f;
    public float idleBreathFrequency = 0.8f;
    public float idleHeadDrift = 1.5f;
    public float blinkIntervalMin = 2.0f;
    public float blinkIntervalMax = 6.0f;
    public float blinkDuration = 0.12f;
    public Sprite blinkSprite;
    public bool useBlinkSprite = false;

    // --- Smoothing ---
    [Header("Smoothing")]
    public float parameterLerpSpeed = 6f;

    [Header("Input Source")]
    public Rigidbody2D rb;
    public float manualSpeed = 0f;

    // --- ANIMATION CURVES ---
    [Header("Animation Curves (The 'Animator')")]
    [Tooltip("Determines Z-rotation over one cycle. X-axis is phase (0-1), Y-axis is multiplier (-1 to 1).")]
    public AnimationCurve frontLegRotationCurve;
    public AnimationCurve backLegRotationCurve;
    public AnimationCurve frontArmRotationCurve;
    public AnimationCurve backArmRotationCurve;
    [Tooltip("Determines bend over one cycle. Y-axis is multiplier (0 to 1).")]
    public AnimationCurve kneeBendCurve;
    public AnimationCurve elbowBendCurve;
    public AnimationCurve footRotationCurve;
    [Tooltip("Determines vertical lift over one cycle. Y-axis is multiplier (0 to 1).")]
    public AnimationCurve legLiftCurve;
    public AnimationCurve bodyBobCurve;
    public AnimationCurve headBobCurve;
    public AnimationCurve neckRotationCurve;
    public AnimationCurve tailBaseRotationCurve;




    [Header("Leg IK Settings")]
    [Tooltip("Master switch to enable/disable leg IK.")]
    public bool useLegIK = true;

    [Tooltip("Layer mask for what the feet should consider 'ground'.")]
    public LayerMask groundLayer;

    [Tooltip("How far down from the foot's ideal position to check for ground.")]
    public float groundCheckDistance = 0.5f;

    [Tooltip("An offset relative to the body to tell the knee which way to bend.")]
    public Vector2 kneePoleTargetOffset = new Vector2(0, -1f); // Point knees slightly downwards/backwards

    [Tooltip("A curve to control how strongly IK takes over during the step cycle. 0 = curves only, 1 = full IK. The X-axis is the step cycle (0-1) and the Y-axis is the IK weight.")]
    public AnimationCurve ikWeightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    // Internal state
    private Vector3 _initialBodyPos, _initialHeadPos, _initialNeckPos, _initialEyesPos;
    private Vector3 _initialFrontLegPos, _initialBackLegPos;
    private float _timer = 0f;
    public enum MoveState { Idle, Walk, Run }
    private MoveState _state = MoveState.Idle;

    // Runtime-smoothed parameters
    private float _currentCycleSpeed, _currentLegSwingAngle, _currentBodyBobAmount, _currentLegLiftAmount;
    private float _currentArmSwingAngle, _currentKneeBendAngle, _currentElbowDragAngle, _currentHeadBobAmount;
    private float _currentNeckSwingAngle, _currentFootAngleMultiplier, _currentTailSwingAngle;

    // Blinking
    private float _nextBlinkTime, _blinkTimer;
    private bool _isBlinking = false;
    private SpriteRenderer _eyesSR;
    private Sprite _originalEyesSprite;
    private Vector3 _originalEyesScale;

    void Start()
    {
        // Store initial local positions
        if (body != null) _initialBodyPos = body.localPosition;
        if (head != null) _initialHeadPos = head.localPosition;
        if (neck != null) _initialNeckPos = neck.localPosition;
        if (eyes != null) _initialEyesPos = eyes.localPosition;
        if (frontLeg != null) _initialFrontLegPos = frontLeg.localPosition;
        if (backLeg != null) _initialBackLegPos = backLeg.localPosition;

        // Initialize all runtime-smoothed params to base walk values (or from walkProfile if present)
        // Ensure profiles exist so we can pull defaults from them
        if (walkProfile == null) walkProfile = new MotionProfile();
        if (idleProfile == null) idleProfile = new MotionProfile();
        if (runProfile == null) runProfile = new MotionProfile();

        // If the inspector didn't set the profiles, seed them from the legacy top-level fields
        // (this keeps your previous serialized values working when upgrading)
        bool walkProfileLooksDefault = Mathf.Approximately(walkProfile.cycleSpeed, 0f) && Mathf.Approximately(walkProfile.legSwingAngle, 0f);
        if (walkProfileLooksDefault)
        {
            walkProfile.cycleSpeed = walkCycleSpeed;
            walkProfile.legSwingAngle = legSwingAngle;
            walkProfile.bodyBobAmount = bodyBobAmount;
            walkProfile.legLiftAmount = legLiftAmount;
            walkProfile.armSwingAngle = armSwingAngle;
            walkProfile.kneeBendAngle = kneeBendAngle;
            walkProfile.elbowDragAngle = elbowDragAngle;
            walkProfile.headBobAmount = headBobAmount;
            walkProfile.neckSwingAngle = neckSwingAngle;
            walkProfile.footAngleMultiplier = footAngleMultiplier;
            walkProfile.tailSwingAngle = tailSwingAngle;

            // Idle defaults (smaller magnitudes)
            idleProfile.cycleSpeed = walkCycleSpeed * 0.3f;
            idleProfile.legSwingAngle = legSwingAngle * 0.05f;
            idleProfile.bodyBobAmount = bodyBobAmount * 0.35f;
            idleProfile.legLiftAmount = legLiftAmount * 0.05f;
            idleProfile.armSwingAngle = armSwingAngle * 0.05f;
            idleProfile.kneeBendAngle = kneeBendAngle * 0.1f;
            idleProfile.elbowDragAngle = elbowDragAngle * 0.1f;
            idleProfile.headBobAmount = headBobAmount * 0.2f;
            idleProfile.neckSwingAngle = neckSwingAngle * 0.2f;
            idleProfile.footAngleMultiplier = footAngleMultiplier * 0.05f;
            idleProfile.tailSwingAngle = tailSwingAngle * 0.2f;

            // Run defaults (multipliers of walk)
            runProfile.cycleSpeed = walkCycleSpeed * runCycleSpeedMultiplier;
            runProfile.legSwingAngle = legSwingAngle * runLegSwingMultiplier;
            runProfile.bodyBobAmount = bodyBobAmount * runBodyBobMultiplier;
            runProfile.legLiftAmount = legLiftAmount * runLegLiftMultiplier;
            runProfile.armSwingAngle = armSwingAngle * runLegSwingMultiplier;
            runProfile.kneeBendAngle = kneeBendAngle * 1.2f;
            runProfile.elbowDragAngle = elbowDragAngle;
            runProfile.headBobAmount = headBobAmount * runBodyBobMultiplier;
            runProfile.neckSwingAngle = neckSwingAngle * 1.1f;
            runProfile.footAngleMultiplier = footAngleMultiplier;
            runProfile.tailSwingAngle = tailSwingAngle * 1.3f;
        }

        // Set runtime smoothed params to the walk profile (starting baseline)
        _currentCycleSpeed = walkProfile.cycleSpeed;
        _currentLegSwingAngle = walkProfile.legSwingAngle;
        _currentBodyBobAmount = walkProfile.bodyBobAmount;
        _currentLegLiftAmount = walkProfile.legLiftAmount;
        _currentArmSwingAngle = walkProfile.armSwingAngle;
        _currentKneeBendAngle = walkProfile.kneeBendAngle;
        _currentElbowDragAngle = walkProfile.elbowDragAngle;
        _currentHeadBobAmount = walkProfile.headBobAmount;
        _currentNeckSwingAngle = walkProfile.neckSwingAngle;
        _currentFootAngleMultiplier = walkProfile.footAngleMultiplier;
        _currentTailSwingAngle = walkProfile.tailSwingAngle;


        // Eye blinking setup
        if (eyes != null)
        {
            _eyesSR = eyes.GetComponent<SpriteRenderer>();
            if (_eyesSR != null)
            {
                _originalEyesSprite = _eyesSR.sprite;
                _originalEyesScale = eyes.localScale;
            }
        }
        _nextBlinkTime = Time.time + Random.Range(blinkIntervalMin, blinkIntervalMax);
    }

    void Update()
    {
        float currentSpeed = rb != null ? Mathf.Abs(rb.velocity.x) : Mathf.Abs(manualSpeed);
        float horizontalInput = rb != null ? rb.velocity.x : manualSpeed;

        // 1. Determine Character State (Idle, Walk, Run)
        if (currentSpeed < 0.1f) _state = MoveState.Idle;
        else if (currentSpeed >= runSpeedThreshold) _state = MoveState.Run;
        else _state = MoveState.Walk;

        // 2. Set Target Animation Parameters Based on State
        // 2. Set Target Animation Parameters Based on State (now profile-driven)
        if (forceState)
        {
            _state = forcedState;
        }
        else
        {
            // Only auto-decide state if not forced
            if (currentSpeed < 0.1f) _state = MoveState.Idle;
            else if (currentSpeed >= runSpeedThreshold) _state = MoveState.Run;
            else _state = MoveState.Walk;
        }

        // Choose profile (fallback to legacy fields if profile is null)
        MotionProfile chosen = walkProfile;
        if (useStateProfiles)
        {
            if (_state == MoveState.Run) chosen = runProfile;
            else if (_state == MoveState.Walk) chosen = walkProfile;
            else chosen = idleProfile;
        }

        float targetCycleSpeed = chosen.cycleSpeed;
        float targetLegSwing = chosen.legSwingAngle;
        float targetBodyBob = chosen.bodyBobAmount;
        float targetLegLift = chosen.legLiftAmount;
        float targetArmSwing = chosen.armSwingAngle;
        float targetKneeBend = chosen.kneeBendAngle;
        float targetElbowDrag = chosen.elbowDragAngle;
        float targetHeadBob = chosen.headBobAmount;
        float targetNeckSwing = chosen.neckSwingAngle;
        float targetFootAngleMult = chosen.footAngleMultiplier;
        float targetTailSwing = chosen.tailSwingAngle;


        // 3. Smoothly Lerp Current Parameters Towards Targets
        _currentCycleSpeed = Mathf.Lerp(_currentCycleSpeed, targetCycleSpeed, Time.deltaTime * parameterLerpSpeed);
        _currentLegSwingAngle = Mathf.Lerp(_currentLegSwingAngle, targetLegSwing, Time.deltaTime * parameterLerpSpeed);
        _currentBodyBobAmount = Mathf.Lerp(_currentBodyBobAmount, targetBodyBob, Time.deltaTime * parameterLerpSpeed);
        _currentLegLiftAmount = Mathf.Lerp(_currentLegLiftAmount, targetLegLift, Time.deltaTime * parameterLerpSpeed);
        _currentArmSwingAngle = Mathf.Lerp(_currentArmSwingAngle, targetArmSwing, Time.deltaTime * parameterLerpSpeed);
        _currentKneeBendAngle = Mathf.Lerp(_currentKneeBendAngle, targetKneeBend, Time.deltaTime * parameterLerpSpeed);
        _currentElbowDragAngle = Mathf.Lerp(_currentElbowDragAngle, targetElbowDrag, Time.deltaTime * parameterLerpSpeed);
        _currentHeadBobAmount = Mathf.Lerp(_currentHeadBobAmount, targetHeadBob, Time.deltaTime * parameterLerpSpeed);
        _currentNeckSwingAngle = Mathf.Lerp(_currentNeckSwingAngle, targetNeckSwing, Time.deltaTime * parameterLerpSpeed);
        _currentFootAngleMultiplier = Mathf.Lerp(_currentFootAngleMultiplier, targetFootAngleMult, Time.deltaTime * parameterLerpSpeed);
        _currentTailSwingAngle = Mathf.Lerp(_currentTailSwingAngle, targetTailSwing, Time.deltaTime * parameterLerpSpeed);

        // 4. Advance Timer and Calculate Cycle Phase (0-1)
        float dirSign = horizontalInput == 0 ? 1f : Mathf.Sign(horizontalInput);
        _timer += Time.deltaTime * _currentCycleSpeed * dirSign;
        float cyclePhase = Mathf.Repeat(_timer / (2f * Mathf.PI), 1f);

        // 5. Handle Idle-Specific Animations (Breathing, Blinking)
        HandleIdleAnimations();

        // 6. Apply Curve-Driven Procedural Animations
        AnimateBodyParts(cyclePhase);

        // 7. Flip Character to Match Movement Direction
        HandleFlipping(horizontalInput);
    }
    private (float, float) SolveTwoBoneIK_2D(Vector2 hipPos, Vector2 kneePos, Vector2 footPos, Vector2 targetPos, Vector2 polePos)
    {
        // Calculate bone lengths
        float upperLegLen = Vector2.Distance(hipPos, kneePos);
        float lowerLegLen = Vector2.Distance(kneePos, footPos);
        float totalLen = upperLegLen + lowerLegLen;

        // Get the vector from the hip to the target
        Vector2 targetDir = targetPos - hipPos;
        float targetDist = Mathf.Min(targetDir.magnitude, totalLen - 0.001f); // Clamp to max reach

        // Law of Cosines to find the angles
        // Angle at the hip
        float cosHipAngle = (targetDist * targetDist + upperLegLen * upperLegLen - lowerLegLen * lowerLegLen) / (2 * targetDist * upperLegLen);
        float hipAngleRad = Mathf.Acos(Mathf.Clamp(cosHipAngle, -1f, 1f));

        // Angle at the knee
        float cosKneeAngle = (upperLegLen * upperLegLen + lowerLegLen * lowerLegLen - targetDist * targetDist) / (2 * upperLegLen * lowerLegLen);
        float kneeAngleRad = Mathf.Acos(Mathf.Clamp(cosKneeAngle, -1f, 1f));
        float kneeAngleDeg = 180f - (kneeAngleRad * Mathf.Rad2Deg); // Knee angle is convex

        // Determine the direction from the hip to the target
        float targetAngleRad = Mathf.Atan2(targetDir.y, targetDir.x);

        // Use the pole target to determine the bend direction (clockwise or counter-clockwise)
        float poleSide = (polePos.x - hipPos.x) * targetDir.y - (polePos.y - hipPos.y) * targetDir.x;
        float bendDirection = Mathf.Sign(poleSide);

        // Combine angles to get the final world-space rotation for the hip
        float finalHipAngleDeg = (targetAngleRad + hipAngleRad * bendDirection) * Mathf.Rad2Deg;

        return (finalHipAngleDeg, kneeAngleDeg * bendDirection);
    }
    private void AnimateBodyParts(float phase)
    {
        // Evaluate curves for all body parts
        float frontLegRot = frontLegRotationCurve.Evaluate(phase) * _currentLegSwingAngle;
        float backLegRot = backLegRotationCurve.Evaluate(phase) * _currentLegSwingAngle;
        float frontArmRot = frontArmRotationCurve.Evaluate(phase) * _currentArmSwingAngle;
        float backArmRot = backArmRotationCurve.Evaluate(phase) * _currentArmSwingAngle;

        float legLift = legLiftCurve.Evaluate(phase) * _currentLegLiftAmount;
        float frontKneeBend = kneeBendCurve.Evaluate(phase) * _currentKneeBendAngle;
        float backKneeBend = kneeBendCurve.Evaluate(phase) * _currentKneeBendAngle; // Can use separate curve if needed
        float elbowDrag = elbowBendCurve.Evaluate(phase) * _currentElbowDragAngle;
        float footRot = footRotationCurve.Evaluate(phase) * _currentLegSwingAngle * _currentFootAngleMultiplier;

        float bodyBob = bodyBobCurve.Evaluate(phase) * _currentBodyBobAmount;
        float headBob = headBobCurve.Evaluate(phase) * _currentHeadBobAmount;
        float neckRot = neckRotationCurve.Evaluate(phase) * _currentNeckSwingAngle;

        float tailRotBase = tailBaseRotationCurve.Evaluate(phase) * _currentTailSwingAngle;
        float tailRotMid = tailBaseRotationCurve.Evaluate(Mathf.Repeat(phase - tailSegmentDelay, 1f)) * _currentTailSwingAngle;
        float tailRotTip = tailBaseRotationCurve.Evaluate(Mathf.Repeat(phase - tailSegmentDelay * 2, 1f)) * _currentTailSwingAngle;

        // Apply transformations
        if (frontLeg) { frontLeg.localRotation = Quaternion.Euler(0, 0, frontLegRot); frontLeg.localPosition = _initialFrontLegPos + new Vector3(0, legLift, 0); }
        if (backLeg) { backLeg.localRotation = Quaternion.Euler(0, 0, backLegRot); backLeg.localPosition = _initialBackLegPos + new Vector3(0, legLift, 0); } // Could use different lift curve/phase
        if (frontLowerLeg) frontLowerLeg.localRotation = Quaternion.Euler(0, 0, frontKneeBend);
        if (backLowerLeg) backLowerLeg.localRotation = Quaternion.Euler(0, 0, backKneeBend);
        if (frontFoot) frontFoot.localRotation = Quaternion.Euler(0, 0, footRot);
        if (backFoot) backFoot.localRotation = Quaternion.Euler(0, 0, -footRot); // Opposite rotation

        if (frontArm) frontArm.localRotation = Quaternion.Euler(0, 0, frontArmRot);
        if (backArm) backArm.localRotation = Quaternion.Euler(0, 0, backArmRot);
        if (frontLowerArm) frontLowerArm.localRotation = Quaternion.Euler(0, 0, elbowDrag);
        if (backLowerArm) backLowerArm.localRotation = Quaternion.Euler(0, 0, elbowDrag);

        if (body && _state != MoveState.Idle) body.localPosition = _initialBodyPos + new Vector3(0, bodyBob, 0);
        if (head && _state != MoveState.Idle) head.localPosition = _initialHeadPos + new Vector3(0, headBob, 0);
        if (neck) neck.localRotation = Quaternion.Euler(0, 0, neckRot);

        if (tailBase) tailBase.localRotation = Quaternion.Euler(0, 0, tailRotBase);
        if (tailMid) tailMid.localRotation = Quaternion.Euler(0, 0, tailRotMid);
        if (tailTip) tailTip.localRotation = Quaternion.Euler(0, 0, tailRotTip);
    }

    private void HandleIdleAnimations()
    {
        if (_state == MoveState.Idle)
        {
            if (body) body.localPosition = Vector3.Lerp(body.localPosition, _initialBodyPos + new Vector3(0, Mathf.Sin(Time.time * idleBreathFrequency) * idleBreathAmount, 0), Time.deltaTime * 5f);
            if (head) head.localRotation = Quaternion.Lerp(head.localRotation, Quaternion.Euler(0, 0, Mathf.Sin(Time.time * idleBreathFrequency * 0.9f) * idleHeadDrift), Time.deltaTime * 5f);

            // Blinking Logic
            if (!_isBlinking && Time.time >= _nextBlinkTime) { _isBlinking = true; _blinkTimer = 0f; }
            if (_isBlinking)
            {
                _blinkTimer += Time.deltaTime;
                if (_blinkTimer >= blinkDuration)
                {
                    _isBlinking = false;
                    _nextBlinkTime = Time.time + Random.Range(blinkIntervalMin, blinkIntervalMax);
                    if (eyes != null) eyes.localScale = _originalEyesScale; // Restore scale
                    if (_eyesSR != null && useBlinkSprite) _eyesSR.sprite = _originalEyesSprite;
                }
                else
                {
                    float t = _blinkTimer / blinkDuration;
                    float closeness = t < 0.5f ? Mathf.Lerp(1f, 0.05f, t * 2f) : Mathf.Lerp(0.05f, 1f, (t - 0.5f) * 2f);
                    if (eyes != null && !useBlinkSprite) eyes.localScale = new Vector3(_originalEyesScale.x, _originalEyesScale.y * closeness, _originalEyesScale.z);
                    if (_eyesSR != null && useBlinkSprite && blinkSprite != null) _eyesSR.sprite = (closeness < 0.5f) ? blinkSprite : _originalEyesSprite;
                }
            }
        }
        else if (_isBlinking) // Abort blink if we start moving
        {
            _isBlinking = false;
            if (eyes != null) eyes.localScale = _originalEyesScale;
            if (_eyesSR != null && useBlinkSprite) _eyesSR.sprite = _originalEyesSprite;
            _nextBlinkTime = Time.time + Random.Range(blinkIntervalMin, blinkIntervalMax);
        }
    }

    private void HandleFlipping(float horizontalInput)
    {
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float moveDirection = Mathf.Sign(horizontalInput);
            if (!artFacesRight) { moveDirection *= -1; }
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * moveDirection, transform.localScale.y, transform.localScale.z);
        }
    }

    // This function runs when the component is first added or reset in the inspector.
    // It pre-populates the AnimationCurves with good default shapes.
        // This function runs when the component is first added or reset in the inspector.
    // It pre-populates the AnimationCurves with good, PERFECTLY LOOPING default shapes.
    void Reset()
    {
        // Define keyframes for a standard sine wave style motion (-1 to 1 and back)
        Keyframe[] sin_keys = new Keyframe[3];
        sin_keys[0] = new Keyframe(0f, -1f, 0f, 0f);
        sin_keys[1] = new Keyframe(0.5f, 1f, 0f, 0f);
        sin_keys[2] = new Keyframe(1f, -1f, 0f, 0f);
        AnimationCurve sinCurve = new AnimationCurve(sin_keys);
        for(int i = 0; i < sin_keys.Length; i++) { sinCurve.SmoothTangents(i, 0f); } // Smooths them out

        // Define keyframes for an inverted sine wave (1 to -1 and back)
        Keyframe[] cos_keys = new Keyframe[3];
        cos_keys[0] = new Keyframe(0f, 1f, 0f, 0f);
        cos_keys[1] = new Keyframe(0.5f, -1f, 0f, 0f);
        cos_keys[2] = new Keyframe(1f, 1f, 0f, 0f);
        AnimationCurve cosCurve = new AnimationCurve(cos_keys);
        for(int i = 0; i < cos_keys.Length; i++) { cosCurve.SmoothTangents(i, 0f); }

        // Define keyframes for a double-hump curve (0 to 1 to 0 to 1 to 0) - good for bobbing
        Keyframe[] bob_keys = new Keyframe[5];
        bob_keys[0] = new Keyframe(0f, 0f, 0f, 0f);
        bob_keys[1] = new Keyframe(0.25f, 1f, 0f, 0f);
        bob_keys[2] = new Keyframe(0.5f, 0f, 0f, 0f);
        bob_keys[3] = new Keyframe(0.75f, 1f, 0f, 0f);
        bob_keys[4] = new Keyframe(1f, 0f, 0f, 0f);
        AnimationCurve bobCurve = new AnimationCurve(bob_keys);
        for(int i = 0; i < bob_keys.Length; i++) { bobCurve.SmoothTangents(i, 0f); }

        // --- Assign the new, perfect curves ---
        frontLegRotationCurve = sinCurve;  // Front leg starts back, moves forward
        backLegRotationCurve = cosCurve;   // Back leg starts forward, moves back
        frontArmRotationCurve = cosCurve;    // Front arm is opposite to front leg
        backArmRotationCurve = sinCurve;     // Back arm is opposite to back leg
        neckRotationCurve = sinCurve;

        kneeBendCurve = bobCurve;
        elbowBendCurve = bobCurve;
        legLiftCurve = bobCurve;
        bodyBobCurve = bobCurve;

        // More specific curves
        headBobCurve = sinCurve; // Head can bob up and down
        footRotationCurve = sinCurve; // Simple foot rotation
        tailBaseRotationCurve = sinCurve; // Tail sways back and forth

        // Initialize state profiles so Reset gives a good starting point in the Inspector
        if (walkProfile == null) walkProfile = new MotionProfile();
        walkProfile.cycleSpeed = walkCycleSpeed;
        walkProfile.legSwingAngle = legSwingAngle;
        walkProfile.bodyBobAmount = bodyBobAmount;
        walkProfile.legLiftAmount = legLiftAmount;
        walkProfile.armSwingAngle = armSwingAngle;
        walkProfile.kneeBendAngle = kneeBendAngle;
        walkProfile.elbowDragAngle = elbowDragAngle;
        walkProfile.headBobAmount = headBobAmount;
        walkProfile.neckSwingAngle = neckSwingAngle;
        walkProfile.footAngleMultiplier = footAngleMultiplier;
        walkProfile.tailSwingAngle = tailSwingAngle;

        if (idleProfile == null) idleProfile = new MotionProfile();
        idleProfile.cycleSpeed = walkCycleSpeed * 0.3f;
        idleProfile.legSwingAngle = legSwingAngle * 0.05f;
        idleProfile.bodyBobAmount = bodyBobAmount * 0.35f;
        idleProfile.legLiftAmount = legLiftAmount * 0.05f;
        idleProfile.armSwingAngle = armSwingAngle * 0.05f;
        idleProfile.kneeBendAngle = kneeBendAngle * 0.1f;
        idleProfile.elbowDragAngle = elbowDragAngle * 0.1f;
        idleProfile.headBobAmount = headBobAmount * 0.2f;
        idleProfile.neckSwingAngle = neckSwingAngle * 0.2f;
        idleProfile.footAngleMultiplier = footAngleMultiplier * 0.05f;
        idleProfile.tailSwingAngle = tailSwingAngle * 0.2f;

        if (runProfile == null) runProfile = new MotionProfile();
        runProfile.cycleSpeed = walkCycleSpeed * runCycleSpeedMultiplier;
        runProfile.legSwingAngle = legSwingAngle * runLegSwingMultiplier;
        runProfile.bodyBobAmount = bodyBobAmount * runBodyBobMultiplier;
        runProfile.legLiftAmount = legLiftAmount * runLegLiftMultiplier;
        runProfile.armSwingAngle = armSwingAngle * runLegSwingMultiplier;
        runProfile.kneeBendAngle = kneeBendAngle * 1.2f;
        runProfile.elbowDragAngle = elbowDragAngle;
        runProfile.headBobAmount = headBobAmount * runBodyBobMultiplier;
        runProfile.neckSwingAngle = neckSwingAngle * 1.1f;
        runProfile.footAngleMultiplier = footAngleMultiplier;
        runProfile.tailSwingAngle = tailSwingAngle * 1.3f;

    }

}
