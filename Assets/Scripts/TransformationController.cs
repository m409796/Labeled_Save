using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformationController : MonoBehaviour
{
    public GameObject playerCharacter; // Reference to the player character
    public GameObject spriteAnimation; // Reference to the GameObject that plays the sprite animation
    public List<SpriteRenderer> renderersToDisable; // List of SpriteRenderers to disable
    public Animator animator; // Reference to the Animator component

    void Start()
    {
        // Ensure the sprite animation is disabled at the start
        spriteAnimation.SetActive(false);
    }

    void Update()
    {
        // For testing: Trigger the transformation with a key press (for example, pressing "T")
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(TransformationSequence());
        }
    }

    private IEnumerator TransformationSequence()
    {
        foreach (var renderer in renderersToDisable)
        {
            renderer.enabled = false;
        }

        spriteAnimation.SetActive(true);

        var animation = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = animation.length;

        yield return new WaitForSeconds(animationLength);

        spriteAnimation.SetActive(false);

        foreach (var renderer in renderersToDisable)
        {
            renderer.enabled = true;
        }
    }
}
