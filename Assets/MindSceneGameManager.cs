using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MindSceneGameManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private Animator animator;
    private static MindSceneGameManager instance;
    public static MindSceneGameManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<MindSceneGameManager>();
            return instance;
        }
    }
    void Start()
    {
        PlayBlinkAnimation();

    }
    public void ShowPanel()
    {
        // Ensure the panel is fully visible at the start
        Color panelColor = fadeImage.color;
        panelColor.a = 1f;  // Set the alpha to 1
        fadeImage.color = panelColor;
    }
    public void PlayBlinkAnimation()
    {
        ShowPanel();
        // Re-enable the Animator
        animator.enabled = true;
        PlayerManager.Instance.canMove = false;

        // Reset the animation state
        animator.Play("blink", 0, 0f); // 0f starts the animation from the beginning
        animator.speed = 1; // Ensure it's playing at normal speed

        // Start coroutine again
        StartCoroutine(WaitForAnimationToEnd());
    }

    private IEnumerator WaitForAnimationToEnd()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99f || animator.IsInTransition(0))
        {
            yield return null;
        }

        animator.speed = 0; // Freeze animation at the last frame

        // Make the panel fully transparent
        Color transparentColor = fadeImage.color;
        transparentColor.a = 0f;
        fadeImage.color = transparentColor;
        PlayerManager.Instance.canMove = true;
        // Disable Animator after the animation ends
        animator.enabled = false;
    }
}
