using UnityEngine;

public class NoiseBackground : MonoBehaviour
{
    public float scrollSpeed = 0.1f;  // Speed of the texture movement
    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Move the texture to create a noise effect
        float offset = Time.time * scrollSpeed;
        material.mainTextureOffset = new Vector2(offset, 0);  // Change the offset to move it horizontally
    }
}
