using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] public Texture2D regularCursor;
    [SerializeField] public Texture2D interactCursor;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private Image blackPanel;  // Reference to the UI Image used for the fade effect
    [SerializeField] private float fadeDuration = 1f; // Duration of the fade effect
    [SerializeField] private GameObject player;

    [SerializeField] private CanvasGroup dialogueBoxCanvasGroup;
    [SerializeField] private CanvasGroup dialogueSmallCanvasGroup;
    [SerializeField] private CanvasGroup ariaHeadCanvasGroup;
    [SerializeField] private CanvasGroup UpCanvasGroup;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeMouuseCursor();
        sceneCameraController.Instance.CinemachineVirtualCamera.Follow = player.transform;
        if (dialogueBoxCanvasGroup != null)
        {
            dialogueBoxCanvasGroup.alpha = 0f;
        }
        if(dialogueSmallCanvasGroup != null)
        {
            dialogueSmallCanvasGroup.alpha = 0f;
        }
        if(ariaHeadCanvasGroup != null)
        {
            ariaHeadCanvasGroup.alpha = 0f;
        }
        if(UpCanvasGroup != null)
        {
            UpCanvasGroup.alpha = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void MovePlayerToPosition(float x, float y)
    {
        if (player != null)
        {
            player.transform.position = new Vector2(x, y);
        }
        else
        {
            Debug.LogWarning("Player reference is not set in GameManager.");
        }
    }
    private void ChangeMouuseCursor()
    {
        Cursor.SetCursor(regularCursor, Vector2.zero, CursorMode.Auto);
    }

    // Public method to initiate the scene transition
    public void TransitionToPosition(Transform targetPosition, GameObject player, GameObject wallToEnable, GameObject wallToDisable)
    {
        StartCoroutine(TransitionCoroutine(targetPosition, player, wallToEnable, wallToDisable));
    }

    // Coroutine to handle the transition with fade effect
    private IEnumerator TransitionCoroutine(Transform targetPosition, GameObject player, GameObject wallToEnable, GameObject wallToDisable)
    {
        PlayerManager.Instance.canMove = false;
        SceneTransition.Instance.canPressTransitionKey = false;
        SceneTransition.Instance.sign.SetActive(false);

        // Ensure the black panel is enabled before starting the fade
        blackPanel.gameObject.SetActive(true);

        // Start fading to black
        yield return StartCoroutine(FadeToBlack());

        // Move the player and manage walls
        if (player != null)
        {
            player.transform.position = targetPosition.position;
        }
        else
        {
            Debug.LogWarning("Player reference is not set in GameManager script.");
        }

        if (wallToEnable != null)
        {
            wallToEnable.SetActive(true);
        }

        if (wallToDisable != null)
        {
            wallToDisable.SetActive(false);
        }

        // Fade back in from black
        yield return StartCoroutine(FadeFromBlack());

        // Disable the black panel after the fade is complete
        blackPanel.gameObject.SetActive(false);

        // Introduce a delay before enabling these elements
        yield return new WaitForSeconds(0.5f); // Adjust the delay duration as needed

        PlayerManager.Instance.canMove = true;
        SceneTransition.Instance.canPressTransitionKey = true;
        SceneTransition.Instance.sign.SetActive(true);
    }


    // Coroutine to fade the screen to black
    private IEnumerator FadeToBlack()
    {
        // Ensure the black panel starts fully transparent
        Color panelColor = blackPanel.color;
        panelColor.a = 0f;  // Set initial alpha to 0
        blackPanel.color = panelColor;

        blackPanel.gameObject.SetActive(true);  // Make sure the panel is visible

        float time = 0;

        // Gradually fade the panel to full black
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            panelColor.a = Mathf.Lerp(0f, 1f, time / fadeDuration);  // Lerp from 0 to 1
            blackPanel.color = panelColor;
            yield return null;
        }

        // Ensure the final alpha is set to 1 (fully opaque)
        panelColor.a = 1f;
        blackPanel.color = panelColor;
    }


    // Coroutine to fade the screen back from black
    private IEnumerator FadeFromBlack()
    {
        // Ensure the black panel starts fully opaque
        Color panelColor = blackPanel.color;
        panelColor.a = 1f;  // Set initial alpha to 1
        blackPanel.color = panelColor;

        float time = 0;

        // Gradually fade the panel to fully transparent
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            panelColor.a = Mathf.Lerp(1f, 0f, time / fadeDuration);  // Lerp from 1 to 0
            blackPanel.color = panelColor;
            yield return null;
        }

        // Ensure the final alpha is set to 0 (fully transparent)
        panelColor.a = 0f;
        blackPanel.color = panelColor;

        blackPanel.gameObject.SetActive(false);  // Optionally deactivate the panel after fading out
    }

}
