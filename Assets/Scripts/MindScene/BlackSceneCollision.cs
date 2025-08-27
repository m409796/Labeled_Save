using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSceneCollision : MonoBehaviour
{
    public Animator animator;
    public Collider2D objectCollider;
    public UnityEngine.UI.Image fadeImage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Adjust tag as needed
        {
            PlayBlinkAnimation();
        }
    }

    public void ShowPanel()
    {
        Debug.Log("efhef");
        Color panelColor = fadeImage.color;
        panelColor.a = 1f;  // Set the alpha to 1
        fadeImage.color = panelColor;
    }

    public void PlayBlinkAnimation()
    {
        ShowPanel();
        animator.enabled = true;
        PlayerManager.Instance.canMove = false;
        animator.Play("blink", 0, 0f);
        animator.speed = 1;

        //StartCoroutine(WaitForAnimationToEnd());
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f || animator.IsInTransition(0))
        {
            yield return null;
        }

        animator.speed = 0;

        // Make the panel fully transparent
        Color transparentColor = fadeImage.color;
        transparentColor.a = 0f;
        fadeImage.color = transparentColor;
        PlayerManager.Instance.canMove = true;

        // Disable Animator and Collider after animation ends
        animator.enabled = false;
        objectCollider.enabled = false;
    }
}