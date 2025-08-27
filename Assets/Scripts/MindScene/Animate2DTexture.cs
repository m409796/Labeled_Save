using UnityEngine;

public class Animate2DTexture : MonoBehaviour
{
    public Material material; // Assign the material with your texture
    public Vector2 scrollSpeed = new Vector2(0.1f, 0.0f); // Adjust for subtle scrolling

    void Update()
    {
        if (material != null)
        {
            Vector2 offset = material.mainTextureOffset; // Get the current offset
            offset += scrollSpeed * Time.deltaTime; // Animate the offset
            material.mainTextureOffset = offset; // Apply the new offset
        }
    }
}
