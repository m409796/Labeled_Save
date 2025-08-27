using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePlayerController : MonoBehaviour
{
    private bool isRunning = false;
    private float lastClickTime = 0f;
    private float doubleClickDelay = 0.3f; // time between clicks to be considered a double click
    private Vector3 targetPosition;
    private float currentSpeed;
    public float walkSpeed = 2;
    public float runSpeed = 5;

    private void Awake()
    {
        targetPosition = transform.position;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;
            if (targetPosition.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (targetPosition.x > transform.position.x)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (Time.time - lastClickTime < doubleClickDelay)
            {
                // double click detected
                isRunning = true;
            }
            else
            {
                // single click
                isRunning = false;
            }

            lastClickTime = Time.time;
        }

        if (isRunning)
        {
            // Run logic goes here
            currentSpeed = runSpeed;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), currentSpeed * Time.deltaTime);
        }
        else
        {
            // Walk logic goes here
            currentSpeed = walkSpeed;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), currentSpeed * Time.deltaTime);
        }
    }
}