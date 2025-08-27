using UnityEngine;
using UnityEngine.Events;

public class EventsTrigger : MonoBehaviour
{
    public UnityEvent onTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onTrigger?.Invoke();
            this.gameObject.SetActive(false);
        }
    }
}
