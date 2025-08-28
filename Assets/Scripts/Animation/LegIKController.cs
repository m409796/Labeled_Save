using UnityEngine;

public class LegIKController : MonoBehaviour
{
    [Tooltip("The IK target this script controls. Should be this transform.")]
    public Transform ikTarget;

    [Tooltip("How high above the foot to start the ground check raycast.")]
    public float stepHeight = 1.0f;

    [Tooltip("How far down to check for ground from the starting point.")]
    public float raycastDistance = 1.5f;

    [Tooltip("A small vertical offset to prevent the foot from clipping into the ground.")]
    public float footOffset = 0.05f;

    [Tooltip("Set this to your 'Ground' layer to ensure we only check for ground.")]
    public LayerMask groundLayer;

    // The original animated position from the Animator
    private Vector3 _animatedPosition;

    void Awake()
    {
        if (ikTarget == null)
        {
            ikTarget = transform;
        }
    }

    // We use LateUpdate to modify the position *after* the Animator has finished its work for the frame.
    void LateUpdate()
    {
        // Store the position set by the animation clip
        _animatedPosition = ikTarget.position;

        // The starting point of our raycast is above the animated foot position
        Vector3 raycastStart = new Vector3(_animatedPosition.x, _animatedPosition.y + stepHeight, _animatedPosition.z);

        // Perform the raycast downwards
        RaycastHit2D hit = Physics2D.Raycast(raycastStart, Vector2.down, raycastDistance, groundLayer);

        // Draw a debug line so we can see what it's doing in the editor
        Debug.DrawLine(raycastStart, raycastStart + Vector3.down * raycastDistance, Color.red);

        if (hit.collider != null)
        {
            // We found ground! Move the IK target to the hit point, plus a small offset.
            ikTarget.position = new Vector3(_animatedPosition.x, hit.point.y + footOffset, _animatedPosition.z);
        }
        else
        {
            // No ground found, so let the foot hang where the animation placed it.
            // We don't need to do anything, it's already at _animatedPosition.
        }
    }
}
