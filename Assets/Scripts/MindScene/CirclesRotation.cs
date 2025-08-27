using UnityEngine;

public class CirclesRotation : MonoBehaviour
{
    [Tooltip("The minimum speed for rotation.")]
    public float minRotationSpeed = 10f;

    [Tooltip("The maximum speed for rotation.")]
    public float maxRotationSpeed = 100f;

    [Tooltip("Set to true for clockwise rotation, false for counterclockwise.")]
    public bool clockwiseRotation = true;

    private float[] rotationSpeeds;

    void Start()
    {
        AssignRandomSpeeds();
    }

    void Update()
    {
        RotateSprites();
    }

    void AssignRandomSpeeds()
    {
        rotationSpeeds = new float[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            rotationSpeeds[i] = Random.Range(minRotationSpeed, maxRotationSpeed);
        }
    }

    void RotateSprites()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.GetComponent<SpriteRenderer>() != null)
            {
                float rotationDirection = clockwiseRotation ? -1f : 1f;

                float rotation = rotationSpeeds[i] * Time.deltaTime * rotationDirection;
                child.localRotation *= Quaternion.Euler(0, 0, rotation);
            }
        }
    }
}
