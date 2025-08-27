using UnityEngine;

public class FollowAriaObject : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Transform player;
    [SerializeField] private float waitDistance = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float wiggleAmplitude = 0.5f;
    [SerializeField] private float wiggleFrequency = 2f;
    [SerializeField] private float waypointProximityThreshold = 0.5f;

    private int currentWaypointIndex = 0;
    private bool isWaiting = true;
    private float wiggleOffset;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned.");
        }

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypoints are not assigned or empty.");
        }

        wiggleOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        if (player == null || waypoints.Length == 0) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (isWaiting && distanceToPlayer <= waitDistance)
        {
            isWaiting = false;
        }

        if (!isWaiting)
        {
            MoveToWaypoint();
        }
    }


    //// in this one after the second waypoint it does not stop and moves infinitly
    //private void MoveToWaypoint()
    //{
    //    if (currentWaypointIndex < waypoints.Length)
    //    {
    //        // Follow waypoints normally
    //        Transform targetWaypoint = waypoints[currentWaypointIndex];
    //        Vector3 targetPosition = targetWaypoint.position;

    //        float wiggle = Mathf.Sin(Time.time * wiggleFrequency + wiggleOffset) * wiggleAmplitude;
    //        Vector3 moveDirection = (targetPosition - transform.position).normalized;
    //        moveDirection.y += wiggle;

    //        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    //        // Check if the object reached the current waypoint
    //        if (Vector3.Distance(transform.position, targetPosition) < waypointProximityThreshold)
    //        {
    //            isWaiting = true;
    //            currentWaypointIndex++;

    //            // If we reach the second waypoint, stop following waypoints
    //            if (currentWaypointIndex == 2)
    //            {
    //                currentWaypointIndex = waypoints.Length; // Exit waypoint logic
    //            }
    //        }
    //    }
    //    else
    //    {
    //        // Move to the right after the second waypoint
    //        Vector3 moveDirection = Vector3.right; // Normalized vector pointing to the right
    //        moveDirection.y += Mathf.Sin(Time.time * wiggleFrequency + wiggleOffset) * wiggleAmplitude;

    //        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    //    }
    //}


    // this one does stop at the second waypoint
    private void MoveToWaypoint()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Follow waypoints normally
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 targetPosition = targetWaypoint.position;

            float wiggle = Mathf.Sin(Time.time * wiggleFrequency + wiggleOffset) * wiggleAmplitude;
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            moveDirection.y += wiggle;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // Check if the object reached the current waypoint
            if (Vector3.Distance(transform.position, targetPosition) < waypointProximityThreshold)
            {
                isWaiting = true;

                // Check if this is the waypoint to stop at
                if (currentWaypointIndex == 1) // Replace '1' with the index of the desired waypoint
                {
                    enabled = false; // Disable the script to stop movement
                    return; // Exit the method to prevent further execution
                }

                currentWaypointIndex++;
            }
        }
    }


    private void OnDrawGizmos()
    {
        // Set Gizmos color for the wait distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, waitDistance);

        // Draw proximity threshold for all waypoints
        if (waypoints != null && waypoints.Length > 0)
        {
            foreach (Transform waypoint in waypoints)
            {
                if (waypoint != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(waypoint.position, waypointProximityThreshold);
                }
            }
        }
    }

}
