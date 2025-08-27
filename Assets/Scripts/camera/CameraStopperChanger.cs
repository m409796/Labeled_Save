using UnityEngine;
using Cinemachine;

public class CameraStopperChanger : MonoBehaviour
{
    public CinemachineConfiner confiner;
    public CinemachineConfiner confinerCutscenes;
    public Collider2D newBoundingShape; // Changed to Collider (not PolygonCollider2D)

    void Start()
    {
        ChangeBoundingShape(newBoundingShape); 
    }

    public void ChangeBoundingShape(Collider2D boundingShape)
    {
        newBoundingShape = boundingShape;
        if (confiner != null && newBoundingShape != null)
        {
            confiner.m_BoundingShape2D = newBoundingShape; // Use m_BoundingVolume instead
            confiner.enabled = false;
            confiner.enabled = true; // Force refresh
            Debug.Log("Bounding shape changed!");
        }
        if(confinerCutscenes!= null && newBoundingShape != null)
        {
            confinerCutscenes.m_BoundingShape2D = newBoundingShape; // Use m_BoundingVolume instead
            confinerCutscenes.enabled = false;
            confinerCutscenes.enabled = true; // Force refresh
            Debug.Log("Bounding shape changed!");
        }
    }
}
