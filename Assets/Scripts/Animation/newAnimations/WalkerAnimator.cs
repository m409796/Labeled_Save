//using UnityEngine;

//public class WalkerAnimator : MonoBehaviour
//{
//    // ... (All fields from the previous version remain the same)
//    [Header("Animation Profile")]
//    [Tooltip("The data asset containing all animation parameters.")]
//    public WalkerAnimationProfile profile;

//    [Header("REQUIRED Body Parts")]
//    public Transform body;
//    public Transform neck;
//    public Transform head;
//    public Transform frontArm;
//    public Transform backArm;
//    public Transform frontLeg;
//    public Transform backLeg;

//    [Header("OPTIONAL Body Parts")]
//    public Transform frontLowerArm;
//    public Transform backLowerArm;
//    public Transform frontLowerLeg;
//    public Transform backLowerLeg;
//    public Transform frontFoot;
//    public Transform backFoot;
//    public Transform eyes;
//    public Transform tailBase;
//    public Transform tailMid;
//    public Transform tailTip;

//    // Internal state
//    private Vector3 _initialBodyPos, _initialHeadPos, _initialNeckPos, _initialEyesPos;
//    private Vector3 _initialFrontLegPos, _initialBackLegPos;
//    private float _timer = 0f;

//    // Runtime-smoothed parameters
//    private float _currentCycleSpeed, _currentLegSwingAngle, _currentBodyBobAmount, _currentLegLiftAmount;
//    private float _currentArmSwingAngle, _currentKneeBendAngle, _currentElbowDragAngle, _currentHeadBobAmount;
//    private float _currentNeckSwingAngle, _currentFootAngleMultiplier, _currentTailSwingAngle;

//    // Blinking
//    private float _nextBlinkTime, _blinkTimer;
//    private bool _isBlinking = false;
//    private SpriteRenderer _eyesSR;
//    private Sprite _originalEyesSprite;
//    private Vector3 _originalEyesScale;

//    // ... (Start(), UpdateAnimation(), GetChosenProfile(), SmoothParameters() methods from the previous version are here)
//    void Start()
//    {
//        if (profile == null)
//        {
//            Debug.LogError($"WalkerAnimator requires an Animation Profile to function!{profile}", this);
//            enabled = false;
//            return;
//        }

//        // Store initial local positions
//        if (body != null) _initialBodyPos = body.localPosition;
//        if (head != null) _initialHeadPos = head.localPosition;
//        if (neck != null) _initialNeckPos = neck.localPosition;
//        if (eyes != null) _initialEyesPos = eyes.localPosition;
//        if (frontLeg != null) _initialFrontLegPos = frontLeg.localPosition;
//        if (backLeg != null) _initialBackLegPos = backLeg.localPosition;

//        InitializeProfiles();
//        InitializeSmoothedParameters();

//        // Eye blinking setup
//        if (eyes != null)
//        {
//            _eyesSR = eyes.GetComponent<SpriteRenderer>();
//            if (_eyesSR != null)
//            {
//                _originalEyesSprite = _eyesSR.sprite;
//                _originalEyesScale = eyes.localScale;
//            }
//        }
//        _nextBlinkTime = Time.time + Random.Range(profile.blinkIntervalMin, profile.blinkIntervalMax);
//    }

//    public void UpdateAnimation(ProceduralWalker.MoveState state, float horizontalInput)
//    {
//        // 1. Set Target Animation Parameters Based on State
//        WalkerAnimationProfile.MotionProfile chosen = GetChosenProfile(state);

//        // 2. Smoothly Lerp Current Parameters Towards Targets
//        SmoothParameters(chosen);

//        // 3. Advance Timer and Calculate Cycle Phase (0-1)
//        float dirSign = horizontalInput == 0 ? 1f : Mathf.Sign(horizontalInput);
//        _timer += Time.deltaTime * _currentCycleSpeed * dirSign;
//        float cyclePhase = Mathf.Repeat(_timer / (2f * Mathf.PI), 1f);

//        // 4. Handle Idle-Specific Animations (Breathing, Blinking)
//        HandleIdleAnimations(state);

//        // 5. Apply Curve-Driven Procedural Animations
//        AnimateBodyParts(cyclePhase, state);
//    }

//    private WalkerAnimationProfile.MotionProfile GetChosenProfile(ProceduralWalker.MoveState state)
//    {
//        if (profile.useStateProfiles)
//        {
//            if (state == ProceduralWalker.MoveState.Run) return profile.runProfile;
//            if (state == ProceduralWalker.MoveState.Walk) return profile.walkProfile;
//            return profile.idleProfile;
//        }
//        return profile.walkProfile; // Fallback to walk if not using profiles
//    }

//    private void SmoothParameters(WalkerAnimationProfile.MotionProfile targetProfile)
//    {
//        float lerpSpeed = profile.parameterLerpSpeed;
//        _currentCycleSpeed = Mathf.Lerp(_currentCycleSpeed, targetProfile.cycleSpeed, Time.deltaTime * lerpSpeed);
//        _currentLegSwingAngle = Mathf.Lerp(_currentLegSwingAngle, targetProfile.legSwingAngle, Time.deltaTime * lerpSpeed);
//        // ... (all other parameter lerps) ...
//    }

//    // --- NEWLY ADDED/COMPLETED METHODS ---

//    private void AnimateBodyParts(float phase, ProceduralWalker.MoveState state)
//    {
//        // Evaluate curves for all body parts
//        float frontLegRot = profile.frontLegRotationCurve.Evaluate(phase) * _currentLegSwingAngle;
//        float backLegRot = profile.backLegRotationCurve.Evaluate(phase) * _currentLegSwingAngle;
//        float frontArmRot = profile.frontArmRotationCurve.Evaluate(phase) * _currentArmSwingAngle;
//        float backArmRot = profile.backArmRotationCurve.Evaluate(phase) * _currentArmSwingAngle;

//        float legLift = profile.legLiftCurve.Evaluate(phase) * _currentLegLiftAmount;
//        float frontKneeBend = profile.kneeBendCurve.Evaluate(phase) * _currentKneeBendAngle;
//        float backKneeBend = profile.kneeBendCurve.Evaluate(phase) * _currentKneeBendAngle;
//        float elbowDrag = profile.elbowBendCurve.Evaluate(phase) * _currentElbowDragAngle;
//        float footRot = profile.footRotationCurve.Evaluate(phase) * _currentLegSwingAngle * _currentFootAngleMultiplier;

//        float bodyBob = profile.bodyBobCurve.Evaluate(phase) * _currentBodyBobAmount;
//        float headBob = profile.headBobCurve.Evaluate(phase) * _currentHeadBobAmount;
//        float neckRot = profile.neckRotationCurve.Evaluate(phase) * _currentNeckSwingAngle;

//        float tailRotBase = profile.tailBaseRotationCurve.Evaluate(phase) * _currentTailSwingAngle;
//        float tailRotMid = profile.tailBaseRotationCurve.Evaluate(Mathf.Repeat(phase - profile.tailSegmentDelay, 1f)) * _currentTailSwingAngle;
//        float tailRotTip = profile.tailBaseRotationCurve.Evaluate(Mathf.Repeat(phase - profile.tailSegmentDelay * 2, 1f)) * _currentTailSwingAngle;

//        // Apply transformations
//        if (frontLeg) { frontLeg.localRotation = Quaternion.Euler(0, 0, frontLegRot); frontLeg.localPosition = _initialFrontLegPos + new Vector3(0, legLift, 0); }
//        if (backLeg) { backLeg.localRotation = Quaternion.Euler(0, 0, backLegRot); backLeg.localPosition = _initialBackLegPos + new Vector3(0, legLift, 0); }
//        if (frontLowerLeg) frontLowerLeg.localRotation = Quaternion.Euler(0, 0, frontKneeBend);
//        if (backLowerLeg) backLowerLeg.localRotation = Quaternion.Euler(0, 0, backKneeBend);
//        if (frontFoot) frontFoot.localRotation = Quaternion.Euler(0, 0, footRot);
//        if (backFoot) backFoot.localRotation = Quaternion.Euler(0, 0, -footRot);

//        if (frontArm) frontArm.localRotation = Quaternion.Euler(0, 0, frontArmRot);
//        if (backArm) backArm.localRotation = Quaternion.Euler(0, 0, backArmRot);
//        if (frontLowerArm) frontLowerArm.localRotation = Quaternion.Euler(0, 0, elbowDrag);
//        if (backLowerArm) backLowerArm.localRotation = Quaternion.Euler(0, 0, elbowDrag);

//        if (body && state != ProceduralWalker.MoveState.Idle) body.localPosition = _initialBodyPos + new Vector3(0, bodyBob, 0);
//        if (head && state != ProceduralWalker.MoveState.Idle) head.localPosition = _initialHeadPos + new Vector3(0, headBob, 0);
//        if (neck) neck.localRotation = Quaternion.Euler(0, 0, neckRot);

//        if (tailBase) tailBase.localRotation = Quaternion.Euler(0, 0, tailRotBase);
//        if (tailMid) tailMid.localRotation = Quaternion.Euler(0, 0, tailRotMid);
//        if (tailTip) tailTip.localRotation = Quaternion.Euler(0, 0, tailRotTip);

//        // IK logic would also be called from here
//    }

//    private void HandleIdleAnimations(ProceduralWalker.MoveState state)
//    {
//        if (state == ProceduralWalker.MoveState.Idle)
//        {
//            if (body) body.localPosition = Vector3.Lerp(body.localPosition, _initialBodyPos + new Vector3(0, Mathf.Sin(Time.time * profile.idleBreathFrequency) * profile.idleBreathAmount, 0), Time.deltaTime * 5f);
//            if (head) head.localRotation = Quaternion.Lerp(head.localRotation, Quaternion.Euler(0, 0, Mathf.Sin(Time.time * profile.idleBreathFrequency * 0.9f) * profile.idleHeadDrift), Time.deltaTime * 5f);

//            // Blinking Logic
//            if (!_isBlinking && Time.time >= _nextBlinkTime) { _isBlinking = true; _blinkTimer = 0f; }
//            if (_isBlinking)
//            {
//                _blinkTimer += Time.deltaTime;
//                if (_blinkTimer >= profile.blinkDuration)
//                {
//                    _isBlinking = false;
//                    _nextBlinkTime = Time.time + Random.Range(profile.blinkIntervalMin, profile.blinkIntervalMax);
//                    if (eyes != null) eyes.localScale = _originalEyesScale;
//                    if (_eyesSR != null && profile.useBlinkSprite) _eyesSR.sprite = _originalEyesSprite;
//                }
//                else
//                {
//                    float t = _blinkTimer / profile.blinkDuration;
//                    float closeness = t < 0.5f ? Mathf.Lerp(1f, 0.05f, t * 2f) : Mathf.Lerp(0.05f, 1f, (t - 0.5f) * 2f);
//                    if (eyes != null && !profile.useBlinkSprite) eyes.localScale = new Vector3(_originalEyesScale.x, _originalEyesScale.y * closeness, _originalEyesScale.z);
//                    if (_eyesSR != null && profile.useBlinkSprite && profile.blinkSprite != null) _eyesSR.sprite = (closeness < 0.5f) ? profile.blinkSprite : _originalEyesSprite;
//                }
//            }
//        }
//        else if (_isBlinking) // Abort blink if we start moving
//        {
//            _isBlinking = false;
//            if (eyes != null) eyes.localScale = _originalEyesScale;
//            if (_eyesSR != null && profile.useBlinkSprite) _eyesSR.sprite = _originalEyesSprite;
//            _nextBlinkTime = Time.time + Random.Range(profile.blinkIntervalMin, profile.blinkIntervalMax);
//        }
//    }

//    private (float, float) SolveTwoBoneIK_2D(Vector2 hipPos, Vector2 kneePos, Vector2 footPos, Vector2 targetPos, Vector2 polePos)
//    {
//        // This function is self-contained and does not need changes
//        float upperLegLen = Vector2.Distance(hipPos, kneePos);
//        float lowerLegLen = Vector2.Distance(kneePos, footPos);
//        float totalLen = upperLegLen + lowerLegLen;
//        Vector2 targetDir = targetPos - hipPos;
//        float targetDist = Mathf.Min(targetDir.magnitude, totalLen - 0.001f);
//        float cosHipAngle = (targetDist * targetDist + upperLegLen * upperLegLen - lowerLegLen * lowerLegLen) / (2 * targetDist * upperLegLen);
//        float hipAngleRad = Mathf.Acos(Mathf.Clamp(cosHipAngle, -1f, 1f));
//        float cosKneeAngle = (upperLegLen * upperLegLen + lowerLegLen * lowerLegLen - targetDist * targetDist) / (2 * upperLegLen * lowerLegLen);
//        float kneeAngleRad = Mathf.Acos(Mathf.Clamp(cosKneeAngle, -1f, 1f));
//        float kneeAngleDeg = 180f - (kneeAngleRad * Mathf.Rad2Deg);
//        float targetAngleRad = Mathf.Atan2(targetDir.y, targetDir.x);
//        float poleSide = (polePos.x - hipPos.x) * targetDir.y - (polePos.y - hipPos.y) * targetDir.x;
//        float bendDirection = Mathf.Sign(poleSide);
//        float finalHipAngleDeg = (targetAngleRad + hipAngleRad * bendDirection) * Mathf.Rad2Deg;
//        return (finalHipAngleDeg, kneeAngleDeg * bendDirection);
//    }

//    void Reset()
//    {
//        Debug.Log("Resetting WalkerAnimator. If you have an Animation Profile assigned, this will populate it with default curves if they are empty.");

//        if (profile == null)
//        {
//            Debug.LogWarning("No Animation Profile assigned. Please create one via Assets > Create > Procedural Walker and assign it here. Curves cannot be generated without a profile asset.", this);
//            return;
//        }

//        Keyframe[] sin_keys = { new Keyframe(0f, -1f), new Keyframe(0.5f, 1f), new Keyframe(1f, -1f) };
//        AnimationCurve sinCurve = new AnimationCurve(sin_keys);
//        for (int i = 0; i < sin_keys.Length; i++) sinCurve.SmoothTangents(i, 0f);

//        Keyframe[] cos_keys = { new Keyframe(0f, 1f), new Keyframe(0.5f, -1f), new Keyframe(1f, 1f) };
//        AnimationCurve cosCurve = new AnimationCurve(cos_keys);
//        for (int i = 0; i < cos_keys.Length; i++) cosCurve.SmoothTangents(i, 0f);

//        Keyframe[] bob_keys = { new Keyframe(0f, 0f), new Keyframe(0.25f, 1f), new Keyframe(0.5f, 0f), new Keyframe(0.75f, 1f), new Keyframe(1f, 0f) };
//        AnimationCurve bobCurve = new AnimationCurve(bob_keys);
//        for (int i = 0; i < bob_keys.Length; i++) bobCurve.SmoothTangents(i, 0f);

//        // Only assign if the curves in the profile are empty/default
//        if (profile.frontLegRotationCurve == null || profile.frontLegRotationCurve.length <= 1) profile.frontLegRotationCurve = sinCurve;
//        if (profile.backLegRotationCurve == null || profile.backLegRotationCurve.length <= 1) profile.backLegRotationCurve = cosCurve;
//        if (profile.frontArmRotationCurve == null || profile.frontArmRotationCurve.length <= 1) profile.frontArmRotationCurve = cosCurve;
//        if (profile.backArmRotationCurve == null || profile.backArmRotationCurve.length <= 1) profile.backArmRotationCurve = sinCurve;
//        if (profile.neckRotationCurve == null || profile.neckRotationCurve.length <= 1) profile.neckRotationCurve = sinCurve;
//        if (profile.kneeBendCurve == null || profile.kneeBendCurve.length <= 1) profile.kneeBendCurve = bobCurve;
//        if (profile.elbowBendCurve == null || profile.elbowBendCurve.length <= 1) profile.elbowBendCurve = bobCurve;
//        if (profile.legLiftCurve == null || profile.legLiftCurve.length <= 1) profile.legLiftCurve = bobCurve;
//        if (profile.bodyBobCurve == null || profile.bodyBobCurve.length <= 1) profile.bodyBobCurve = bobCurve;
//        if (profile.headBobCurve == null || profile.headBobCurve.length <= 1) profile.headBobCurve = sinCurve;
//        if (profile.footRotationCurve == null || profile.footRotationCurve.length <= 1) profile.footRotationCurve = sinCurve;
//        if (profile.tailBaseRotationCurve == null || profile.tailBaseRotationCurve.length <= 1) profile.tailBaseRotationCurve = sinCurve;

//        // The logic for initializing profiles was moved to Start() to ensure it runs correctly at runtime.
//    }

//    // ... (Initialization methods from the previous version are here)
//    #region Initialization
//    private void InitializeProfiles()
//    {
//        // ... (this method remains the same)
//    }

//    private void InitializeSmoothedParameters()
//    {
//        // ... (this method remains the same)
//    }
//    #endregion
//}