//// WalkerAnimationProfile.cs
//using UnityEngine;

//[CreateAssetMenu(fileName = "New Walker Profile", menuName = "Procedural Walker/Animation Profile")]
//public class WalkerAnimationProfile : ScriptableObject
//{
//    [Header("Art and Direction")]
//    [Tooltip("Set this to TRUE if your original sprite art faces to the right.")]
//    public bool artFacesRight = true;

//    [Header("Animation Amplitudes (Base defaults - used to initialize profiles)")]
//    public float walkCycleSpeed = 5.5f;
//    public float bodyBobAmount = 0.04f;
//    public float armSwingAngle = 20f;
//    public float legSwingAngle = 25f;
//    public float kneeBendAngle = 45f; 
//    public float elbowDragAngle = -20f;
//    public float legLiftAmount = 0.05f;
//    public float headBobAmount = 0.04f;
//    public float neckSwingAngle = 10f;
//    public float footAngleMultiplier = 0.5f;
//    public float tailSwingAngle = 25f;
//    public float tailSegmentDelay = 0.15f;

//    [Header("Run Multipliers (Legacy)")]
//    public float runSpeedThreshold = 5f;
//    public float runCycleSpeedMultiplier = 1.6f;
//    public float runLegSwingMultiplier = 1.5f;
//    public float runBodyBobMultiplier = 1.15f;
//    public float runLegLiftMultiplier = 1.2f;

//    [System.Serializable]
//    public class MotionProfile
//    {
//        [Header("Timing / Magnitudes")]
//        public float cycleSpeed = 5.5f;
//        public float legSwingAngle = 25f;
//        public float bodyBobAmount = 0.04f;
//        public float legLiftAmount = 0.05f;
//        public float armSwingAngle = 20f;
//        public float kneeBendAngle = 45f;
//        public float elbowDragAngle = -20f;
//        public float headBobAmount = 0.04f;
//        public float neckSwingAngle = 10f;
//        public float footAngleMultiplier = 0.5f;
//        public float tailSwingAngle = 25f;
//    }

//    [Header("State Profiles (Idle / Walk / Run)")]
//    public MotionProfile idleProfile;
//    public MotionProfile walkProfile;
//    public MotionProfile runProfile;

//    [Header("State Control")]
//    [Tooltip("If true, uses the profiles above for per-state parameters")]
//    public bool useStateProfiles = true;

//    [Header("Idle / Blinking")]
//    public float idleBreathAmount = 0.02f;
//    public float idleBreathFrequency = 0.8f;
//    public float idleHeadDrift = 1.5f;
//    public float blinkIntervalMin = 2.0f;
//    public float blinkIntervalMax = 6.0f;
//    public float blinkDuration = 0.12f;
//    public Sprite blinkSprite;
//    public bool useBlinkSprite = false;

//    [Header("Smoothing")]
//    public float parameterLerpSpeed = 6f;

//    [Header("Animation Curves")]
//    public AnimationCurve frontLegRotationCurve;
//    public AnimationCurve backLegRotationCurve;
//    public AnimationCurve frontArmRotationCurve;
//    public AnimationCurve backArmRotationCurve;
//    public AnimationCurve kneeBendCurve;
//    public AnimationCurve elbowBendCurve;
//    public AnimationCurve footRotationCurve;
//    public AnimationCurve legLiftCurve;
//    public AnimationCurve bodyBobCurve;
//    public AnimationCurve headBobCurve;
//    public AnimationCurve neckRotationCurve;
//    public AnimationCurve tailBaseRotationCurve;

//    [Header("Leg IK Settings")]
//    public bool useLegIK = true;
//    public LayerMask groundLayer;
//    public float groundCheckDistance = 0.5f;
//    public Vector2 kneePoleTargetOffset = new Vector2(0, -1f);
//    public AnimationCurve ikWeightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
//}
