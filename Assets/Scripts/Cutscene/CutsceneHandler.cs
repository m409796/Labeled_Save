using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutsceneHandler : MonoBehaviour
{
    private static CutsceneHandler instance;
    public static CutsceneHandler Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<CutsceneHandler>();
            return instance;
        }
    }
    public Camera cam;
    public CinemachineVirtualCamera vCam;
    private CutsceneElementBase[] cutsceneElements;
    private int index = -1;

    public delegate void CutSceneStarted();
    public static CutSceneStarted cutSceneStarted;

    public delegate void CutSceneEnded();
    public static CutSceneStarted cutSceneEnded;

    private float originalFOV;
    public GameObject playerFollowPos;

    public void Start()
    {
        cutsceneElements = GetComponents<CutsceneElementBase>();
    }

    private void ExecuteCurrentElement()
    {
        if (index >= 0 && index < cutsceneElements.Length)
            cutsceneElements[index].Execute();
    }

    public void PlayNextElement()
    {
        index++;
        if (index >= cutsceneElements.Length)
        {
            //Debug.Log("destroy");
            cutSceneEnded?.Invoke();
            //Destroy(gameObject);
        }
        else
        {
            if (index > 0)
                cutsceneElements[index - 1].enabled = false;
            ExecuteCurrentElement();
        }

    }
    public void ManualEndCutscene()
    {
        // Store the original FOV/Orthographic Size and player position
        float originalFOV = vCam.m_Lens.FieldOfView; // Store the original FOV if it's a perspective camera
        float originalOrthographicSize = vCam.m_Lens.OrthographicSize; // Store the original Orthographic Size if it's an orthographic camera

        // Set the player position directly (assuming you have the player reference already)
        Transform playerTransform = playerFollowPos.transform; // Replace with your actual player transform reference

        // Start the camera transition back to the player
        StartCoroutine(ReturnToPlayer(playerTransform.position, originalFOV, originalOrthographicSize));

        // Hide letterbox effect
        CSE_CameraZoom.Instance.hideLetterbox();
    }



    private IEnumerator ReturnToPlayer(Vector3 playerPosition, float originalFOV, float originalOrthographicSize)
    {
        Vector3 originalPosition = vCam.transform.position;
        float startTime = Time.time;
        float elapsedTime = 0;
        float duration = 1.0f; // Duration of the transition back

        bool isOrthographic = vCam.m_Lens.Orthographic;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 newPosition = Vector3.Lerp(originalPosition, playerPosition, t);

            // Set the camera's Z position to -10
            newPosition.z = -10;

            vCam.transform.position = newPosition;

            if (isOrthographic)
            {
                vCam.m_Lens.OrthographicSize = Mathf.Lerp(vCam.m_Lens.OrthographicSize, originalOrthographicSize, t);
            }
            else
            {
                vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, originalFOV, t);
            }

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        // Finalize the camera position with Z set to -10 and reset FOV/Orthographic Size
        Vector3 finalPosition = playerPosition;
        finalPosition.z = -10; // Set the final Z position to -10
        vCam.transform.position = finalPosition;

        if (isOrthographic)
        {
            vCam.m_Lens.OrthographicSize = originalOrthographicSize;
        }
        else
        {
            vCam.m_Lens.FieldOfView = originalFOV;
        }

        // Do not assign the Follow target to the player.
        // The camera will simply stop following the player and be at their position.

        // Continue to the next cutscene element or end the cutscene
        PlayNextElement();
    }
}
