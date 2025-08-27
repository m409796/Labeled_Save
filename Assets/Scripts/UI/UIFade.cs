using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public static UIFade Instance { get; private set; }

    public Image fadingImage;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float fadeSpeedUnBlack = 1f;

    private bool shouldTurnBlack;
    private bool shouldTurnUnblack;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (fadingImage != null)
        {
            fadingImage.color = new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b, 0f);
        }
        else
        {
            Debug.LogError("fadingImage is not assigned in UIFade!");
        }

    }
    private void Update()
    {
        if (shouldTurnBlack)
        {
            fadingImage.color = new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b,
                Mathf.MoveTowards(fadingImage.color.a, 1f, fadeSpeed * Time.deltaTime));

            if (fadingImage.color.a >= 0.99f)
            {
                shouldTurnBlack = false;
            }
        }

        if (shouldTurnUnblack)
        {
            fadingImage.color = new Color(fadingImage.color.r, fadingImage.color.g, fadingImage.color.b,
                Mathf.MoveTowards(fadingImage.color.a, 0f, fadeSpeedUnBlack * Time.deltaTime));

            if (fadingImage.color.a <= 0.01f)
            {
                shouldTurnUnblack = false;
            }
        }
    }

    public void BlackTurner()
    {
        shouldTurnBlack = true;
        shouldTurnUnblack = false;
    }

    public void UnBlackTurner()
    {
        shouldTurnBlack = false;
        shouldTurnUnblack = true;
    }
}
