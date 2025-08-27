using UnityEngine;
using EasyTransition;

public class SceneChangeTrigger : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    //[SerializeField] private float waitToLoad = 1f; // Wait time before moving player
    //private bool shouldLoadAfterFade;
    public TransitionSettings myTransitionSettings;
    public GameObject player;
    public GameObject sceneToDisable;
    public GameObject sceneToEnable;
    public float startDelay;

    private void Update()
    {
        //if (shouldLoadAfterFade)
        //{
        //    waitToLoad -= Time.deltaTime;
        //    if (waitToLoad <= 0)
        //    {
        //        shouldLoadAfterFade = false;
        //        GameManager.Instance.MovePlayerToPosition(spawnPoint.position.x, spawnPoint.position.y);
        //        UIFade.Instance.UnBlackTurner(); // Fade back in
        //        Time.timeScale = 1f; // Reset time to normal immediately
        //        Time.fixedDeltaTime = 0.02f;
        //    }
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Call this when you want to move the player
            //sceneToDisable.SetActive(false);
            //sceneToEnable.SetActive(true);
            PlayerManager.Instance.canMove = false;
            TransitionManager.Instance.Transitions(myTransitionSettings, startDelay, player, spawnPoint.position);
            
            //shouldLoadAfterFade = true;
            //UIFade.Instance.BlackTurner(); // Start fade-out
        }
    }
}
