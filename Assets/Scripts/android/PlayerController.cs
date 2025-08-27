using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float threshold = 0.2f;  // Time window for detecting simultaneous clicks
    public float moveSpeed = 3.0f;
    public float runSpeed = 6.0f;

    private bool isWalking = false;
    private bool isRunning = false;
    private float lastClickTime = -10.0f;
    private int clickCount = 0;

    void Update()
    {
        if (isWalking)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        if (isRunning)
        {
            transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
        }

        // Check for user input
        if (Input.GetMouseButtonDown(0))
        {
            // Increment the click count
            clickCount++;

            if (clickCount == 1)
            {
                // Record the time of the first click
                lastClickTime = Time.time;
                isWalking = true;
                isRunning = false;
            }
            else if (clickCount == 2)
            {
                // Check if the second click is within the threshold
                if (Time.time - lastClickTime < threshold)
                {
                    isWalking = false;
                    isRunning = true;
                }
                clickCount = 0; // Reset the click count after detecting a double-click
            }
        }

        // Check if we need to stop the player
        if (isWalking || isRunning)
        {
            if (Time.time - lastClickTime >= threshold)
            {
                isWalking = false;
                isRunning = false;
                clickCount = 0; // Reset the click count if time exceeds threshold
            }
        }
    }
}