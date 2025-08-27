/*
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
    private float doubleClickDelay = 1f;


    private void Update()
    {
        if (canMove && Input.GetMouseButtonDown(0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;
            currentSpeed = walkingSpeed;
            if (Time.time - lastClickTime < doubleClickDelay)
            {
                currentSpeed = runningSpeed;
                isWalking = false;
                isRunning = true;
                Debug.Log("why");
            }
            else
            {
                currentSpeed = walkingSpeed;
                isRunning = false;
                isWalking = true;
                Debug.Log("here");
            }
            if (isRunning && Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastClickTime < doubleClickDelay)
                {
                    currentSpeed = runningSpeed;
                    isRunning = true;
                    isWalking = false;
                    Debug.Log("Double-clicked!");
                }
                else
                {
                    // here is waiting for the second click
                    currentSpeed = walkingSpeed;
                    isRunning = false;
                    isWalking = true;
                }
            }
        }

        lastClickTime = Time.time;


        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), currentSpeed * Time.deltaTime);

        if (targetPosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (targetPosition.x > transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}








*/