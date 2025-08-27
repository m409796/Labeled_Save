using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapAnimationsController : MonoBehaviour
{
    public GameObject playerCharacter; // Reference to the player character
    public GameObject spriteAnimation; // Reference to the GameObject that plays the sprite animation (transformation)
    public GameObject walkingAnimation; // Reference to the GameObject that plays the walking animation
    public List<SpriteRenderer> renderersToDisable; // List of SpriteRenderers to disable on playerCharacter
    public Animator animator; // Reference to the Animator component for transformation

    void Start()
    {
        // Ensure both animations are disabled at the start
        //spriteAnimation.SetActive(false);
        walkingAnimation.SetActive(false);
    }

    void Update()
    {
        // For testing: Trigger the transformation with a key press (for example, pressing "T")
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(TransformationSequence());
        }

        // Update walking animation position if it's active
        if (walkingAnimation.activeSelf)
        {
            UpdateWalkingAnimationPosition();
        }
    }

    // Transformation sequence
    private IEnumerator TransformationSequence()
    {
        // Disable idle sprite renderers
        foreach (var renderer in renderersToDisable)
        {
            renderer.enabled = false;
        }

        // Play the transformation animation
        //spriteAnimation.SetActive(true);

        // Wait for the transformation animation to complete
        var animation = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = animation.length;
        yield return new WaitForSeconds(animationLength);

        // End the transformation animation
        //spriteAnimation.SetActive(false);

        // Re-enable idle sprite renderers
        foreach (var renderer in renderersToDisable)
        {
            renderer.enabled = true;
        }
    }

    // This method controls the walking animation
    public void ToggleWalkingAnimation(bool isWalking)
    {
        if (isWalking)
        {
            // Disable idle sprite renderers and play walking animation
            foreach (var renderer in renderersToDisable)
            {
                renderer.enabled = false;
            }

            // Update position of the walking animation to match the player's x position only
            UpdateWalkingAnimationPosition();
            walkingAnimation.SetActive(true);
        }
        else
        {
            // Re-enable idle sprite renderers and stop walking animation
            foreach (var renderer in renderersToDisable)
            {
                renderer.enabled = true;
            }
            walkingAnimation.SetActive(false);
        }
    }

    // This method updates the position of the walking animation to match the player's x position only
    private void UpdateWalkingAnimationPosition()
    {
        if (walkingAnimation != null && playerCharacter != null)
        {
            Vector3 walkingAnimPosition = walkingAnimation.transform.position;
            walkingAnimPosition.x = playerCharacter.transform.position.x;
            walkingAnimation.transform.position = walkingAnimPosition;
        }
    }
}