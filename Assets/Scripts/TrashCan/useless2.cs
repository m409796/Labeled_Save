/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkingSpeed;
    public float runningSpeed;
    private float currentSpeed;
    private Vector3 targetPosition;
    private bool canMove = true;
    private bool isWalking;
    private bool isRunning;
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.5f;

    void Update()
    {
        if (canMove && Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;
            if (!isWalking && !isRunning)
            {
                // First movement, starts with walking
                currentSpeed = walkingSpeed;
                isWalking = true;
            }
            if (Time.time - lastClickTime < doubleClickDelay)
            {
                // Double-clicked
                if (!isWalking && !isRunning)
                {
                    // First movement, starts with walking
                    currentSpeed = walkingSpeed;
                    isWalking = true;
                }
                else
                {
                    if (isRunning)
                    {
                        currentSpeed = runningSpeed;
                    }
                    else
                    {
                        currentSpeed = walkingSpeed;
                    }

                    isRunning = false;
                    isWalking = !isRunning;
                }
            }
            else
            {
                // Single click, start walking
                currentSpeed = walkingSpeed;
                isWalking = true;
                isRunning = false;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), currentSpeed * Time.deltaTime);

        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (transform.position.x == targetPosition.x)
        {
            // Reached target, stop moving
            isWalking = false;
            isRunning = false;
        }

        lastClickTime = Time.time;
    }
}

*/