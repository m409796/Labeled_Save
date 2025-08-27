using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CS_Manager : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera; // The main camera following the player
    [SerializeField] private CinemachineVirtualCamera cutsceneCamera;
    [SerializeField] private Camera CameraProjection;


    void Start()
    {
        CutsceneHandler.cutSceneEnded += OnCutsceneEnded;
    }

    void Update()
    {
        
    }

    private void OnCutsceneEnded()
    {
        Debug.Log("Cutscene has ended! Resuming gameplay...");
        cutsceneCamera.Priority = 0;
        cutsceneCamera.Follow = CutsceneHandler.Instance.playerFollowPos.transform;


        CutsceneHandler.Instance.StartCoroutine(WaitForCameraSize(5.3f));
    }
    private IEnumerator WaitForCameraSize(float targetSize)
    {
        // Wait until the camera size is close enough to the target
        while (Mathf.Abs(CameraProjection.orthographicSize - targetSize) > 0.1f)
        {
            yield return null; // Wait for the next frame
        }

        PlayerManager.Instance.canMove = true;
    }
    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        CutsceneHandler.cutSceneEnded -= OnCutsceneEnded;
    }
}
