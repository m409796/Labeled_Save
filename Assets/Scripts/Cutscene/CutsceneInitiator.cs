using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneInitiator : MonoBehaviour
{
    private CutsceneHandler cutsceneHandler;
    private BoxCollider2D collider2d;

    private void Start()
    {
        cutsceneHandler = GetComponent<CutsceneHandler>();
        collider2d = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CutsceneHandler.cutSceneStarted?.Invoke();
            cutsceneHandler.PlayNextElement();
        }

    }
}