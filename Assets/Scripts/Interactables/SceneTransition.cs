using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition instance;
    public static SceneTransition Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<SceneTransition>();
            return instance;
        }
    }
    public bool canPressTransitionKey ;
    public GameObject sign;


    public GameObject player; // Reference to the player GameObject

    // Function to move the player to a specified position and enable/disable game objects with fade effect
    public void TransitionToPosition(Transform targetPosition, GameObject wallToEnable, GameObject wallToDisable)
    {
        GameManager.Instance.TransitionToPosition(targetPosition, player, wallToEnable, wallToDisable);
    }
}
