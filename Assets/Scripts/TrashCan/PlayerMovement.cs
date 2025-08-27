using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Speed of the player's movement
    public float speed = 5f;

    // Update is called once per frame
    void Update()
    {
        // Get the horizontal input (left/right arrow keys or A/D keys)
        float moveInput = Input.GetAxis("Horizontal");

        // Calculate the new position
        Vector3 newPosition = transform.position + new Vector3(moveInput * speed * Time.deltaTime, 0f, 0f);

        // Update the player's position
        transform.position = newPosition;
    }
}