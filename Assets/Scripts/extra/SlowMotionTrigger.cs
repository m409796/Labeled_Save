using UnityEngine;
using UnityEngine.Events;

public class SlowMotionTrigger : MonoBehaviour
{
    public float slowMotionScale = 0.3f; 
    public float slowMotionDuration = 2f;
    public UnityEvent onSlowMotionEnd;
    public GameObject Blocker;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Time.timeScale = slowMotionScale; 
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Invoke(nameof(ResetTimeScale), slowMotionDuration);
            Blocker.SetActive(true);
        }
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f; 
        Time.fixedDeltaTime = 0.02f;
        Blocker.SetActive(false);
        onSlowMotionEnd?.Invoke();
    }
}
