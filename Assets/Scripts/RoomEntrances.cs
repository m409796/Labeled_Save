using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntrances : SceneTransition
{
    private bool playerInside = false;
    
    [SerializeField] GameObject sceneToEnable;
    [SerializeField] GameObject sceneToDisable;
    [SerializeField] Transform targetPos;


    void Start()
    {
        if (sign != null)
        {
            sign.SetActive(false);
        }
        canPressTransitionKey = true;
    }

    void Update()
    {
        if (playerInside && canPressTransitionKey&& Input.GetKeyDown(KeyCode.F))
        {
            TransitionToPosition(targetPos, sceneToEnable, sceneToDisable);
            PlayerManager.Instance.canMove = false;
            canPressTransitionKey = false;
            sign.SetActive(false);
        }
        //Debug.Log(playerInside);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered: " + other.name);
            playerInside = true; 
            if (sign != null /*&& PlayerManager.Instance.canMove*/)
            {
                sign.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exited: " + other.name);
            playerInside = false; 
            if (sign != null)
            {
                sign.SetActive(false);
            }
        }
    }
}
