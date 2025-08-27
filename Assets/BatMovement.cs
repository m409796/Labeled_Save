using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatMovement : MonoBehaviour
{
    public Transform[] waypoints; // Array of waypoints
    public float speed = 2f; // Speed of bat movement
    private int currentWaypointIndex = 0; // Index of the current waypoint

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component to flip the bat
        transform.position = waypoints[0].position; // Start at the first waypoint
    }

    void Update()
    {
        MoveToNextWaypoint();
    }

    void MoveToNextWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Check if the bat has reached the current waypoint
        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;

            // Flip the bat when reaching waypoints 3 and 4
            if (currentWaypointIndex == 3 || currentWaypointIndex == 4)
            {
                Flip();
            }

            // If the bat reaches the last waypoint, loop back to the first waypoint
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Go back to the first waypoint
            }
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Reverse the X scale to flip the bat
        transform.localScale = localScale;
    }
}
