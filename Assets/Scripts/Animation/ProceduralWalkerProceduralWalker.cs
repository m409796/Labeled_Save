// Filename: ProceduralWalker.cs
using UnityEngine;

public class ProceduralWalker : MonoBehaviour
{
    [Header("REQUIRED Body Parts")]
    public Transform body;
    public Transform neck;
    public Transform head;
    public Transform frontArm;    // Assign the Front_Upper_Arm
    public Transform backArm;     // Assign the Back_Upper_Arm
    public Transform frontLeg;    // Assign the Front_Upper_Leg
    public Transform backLeg;     // Assign the Back_Upper_Leg

    [Header("OPTIONAL Body Parts")]
    public Transform frontLowerArm; // NEW: For elbow bend
    public Transform backLowerArm;  // NEW: For elbow bend
    public Transform frontLowerLeg; // NEW: For knee bend
    public Transform backLowerLeg;  // NEW: For knee bend
    public Transform frontFoot;
    public Transform backFoot;
    public Transform eyes;
    public Transform tailBase;
    public Transform tailMid;
    public Transform tailTip;

    [Header("Animation Parameters")]
    public float walkCycleSpeed = 6f;
    public float bodyBobAmount = 0.08f;
    public float armSwingAngle = 40f;
    public float legSwingAngle = 50f;
    public float lowerLimbAngle = 45f; // NEW: Controls the bend of knees/elbows
    public float headBobAmount = 0.04f;
    public float neckSwingAngle = 10f;
    public float footAngleMultiplier = 0.5f;
    public float tailSwingAngle = 25f;
    public float tailSegmentDelay = 0.2f;

    [Header("Input Source")]
    [Tooltip("Connect your Rigidbody2D to read speed automatically")]
    public Rigidbody2D rb;
    [Tooltip("Or set speed manually if not using a Rigidbody2D")]
    public float manualSpeed = 0f;

    private Vector3 _initialBodyPos, _initialHeadPos, _initialNeckPos, _initialEyesPos;
    private float _timer = 0f;

    void Start()
    {
        if (body != null) _initialBodyPos = body.localPosition;
        if (head != null) _initialHeadPos = head.localPosition;
        if (neck != null) _initialNeckPos = neck.localPosition;
        if (eyes != null) _initialEyesPos = eyes.localPosition;
    }

    void Update()
    {
        float currentSpeed = rb != null ? Mathf.Abs(rb.velocity.x) : Mathf.Abs(manualSpeed);
        float horizontalInput = rb != null ? rb.velocity.x : manualSpeed;

        if (currentSpeed < 0.1f)
        {
            _timer = 0;
            // Smoothly lerp all parts back to their neutral positions (including new lower limbs)
            if (body) body.localPosition = Vector3.Lerp(body.localPosition, _initialBodyPos, Time.deltaTime * 10f);
            if (neck) neck.localRotation = Quaternion.Lerp(neck.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (head) head.localPosition = Vector3.Lerp(head.localPosition, _initialHeadPos, Time.deltaTime * 10f);
            if (eyes) eyes.localPosition = Vector3.Lerp(eyes.localPosition, _initialEyesPos, Time.deltaTime * 10f);
            if (frontArm) frontArm.localRotation = Quaternion.Lerp(frontArm.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (backArm) backArm.localRotation = Quaternion.Lerp(backArm.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (frontLowerArm) frontLowerArm.localRotation = Quaternion.Lerp(frontLowerArm.localRotation, Quaternion.identity, Time.deltaTime * 10f); // NEW
            if (backLowerArm) backLowerArm.localRotation = Quaternion.Lerp(backLowerArm.localRotation, Quaternion.identity, Time.deltaTime * 10f);   // NEW
            if (frontLeg) frontLeg.localRotation = Quaternion.Lerp(frontLeg.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (backLeg) backLeg.localRotation = Quaternion.Lerp(backLeg.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (frontLowerLeg) frontLowerLeg.localRotation = Quaternion.Lerp(frontLowerLeg.localRotation, Quaternion.identity, Time.deltaTime * 10f); // NEW
            if (backLowerLeg) backLowerLeg.localRotation = Quaternion.Lerp(backLowerLeg.localRotation, Quaternion.identity, Time.deltaTime * 10f);   // NEW
            if (frontFoot) frontFoot.localRotation = Quaternion.Lerp(frontFoot.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (backFoot) backFoot.localRotation = Quaternion.Lerp(backFoot.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (tailBase) tailBase.localRotation = Quaternion.Lerp(tailBase.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (tailMid) tailMid.localRotation = Quaternion.Lerp(tailMid.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            if (tailTip) tailTip.localRotation = Quaternion.Lerp(tailTip.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            return;
        }

        _timer += Time.deltaTime * walkCycleSpeed;

        float swing = Mathf.Sin(_timer);
        float swingOffset = Mathf.Sin(_timer + Mathf.PI / 2f); // An offset swing for more natural movement

        // --- Limb Animation (UPDATED) ---
        // Upper Limbs
        if (frontLeg) frontLeg.localRotation = Quaternion.Euler(0, 0, swing * legSwingAngle);
        if (backLeg) backLeg.localRotation = Quaternion.Euler(0, 0, -swing * legSwingAngle);
        if (frontArm) frontArm.localRotation = Quaternion.Euler(0, 0, -swing * armSwingAngle);
        if (backArm) backArm.localRotation = Quaternion.Euler(0, 0, swing * armSwingAngle);

        // NEW: Lower Limbs (Knees and Elbows)
        float lowerLimbBend = (swingOffset * 0.5f + 0.5f) * lowerLimbAngle; // Creates a 0-1 range bend
        if (frontLowerLeg) frontLowerLeg.localRotation = Quaternion.Euler(0, 0, lowerLimbBend);
        if (backLowerLeg) backLowerLeg.localRotation = Quaternion.Euler(0, 0, lowerLimbBend);
        if (frontLowerArm) frontLowerArm.localRotation = Quaternion.Euler(0, 0, lowerLimbBend);
        if (backLowerArm) backLowerArm.localRotation = Quaternion.Euler(0, 0, lowerLimbBend);

        // Feet
        if (frontFoot) frontFoot.localRotation = Quaternion.Euler(0, 0, swing * (legSwingAngle * footAngleMultiplier));
        if (backFoot) backFoot.localRotation = Quaternion.Euler(0, 0, -swing * (legSwingAngle * footAngleMultiplier));

        // --- Body, Head, and Tail ---
        float bodyBob = Mathf.Abs(swing) * bodyBobAmount;
        float headBob = Mathf.Sin(_timer * 2f) * headBobAmount;
        float neckSwing = Mathf.Sin(_timer + Mathf.PI / 2f) * neckSwingAngle;

        if (body) body.localPosition = _initialBodyPos + new Vector3(0, bodyBob, 0);
        if (neck) neck.localRotation = Quaternion.Euler(0, 0, neckSwing);
        if (head) head.localPosition = _initialHeadPos + new Vector3(0, headBob, 0);

        if (tailBase) tailBase.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(_timer) * tailSwingAngle);
        if (tailMid) tailMid.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(_timer - tailSegmentDelay) * tailSwingAngle);
        if (tailTip) tailTip.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(_timer - tailSegmentDelay * 2) * tailSwingAngle);

        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            float direction = Mathf.Sign(horizontalInput);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
        }
    }
}
