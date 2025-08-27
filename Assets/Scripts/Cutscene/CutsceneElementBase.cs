using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneElementBase : MonoBehaviour
{
    public float duration;
    public CutsceneHandler cutsceneHandler { get; private set; }

    private void Start()
    {
        cutsceneHandler = GetComponent<CutsceneHandler>();
    }

    public virtual void Execute()
    {
    }

    protected IEnumerator WaitAndAdvance()
    {
        yield return new WaitForSeconds(duration);
        cutsceneHandler.PlayNextElement();
    }
}
// Proceed to next cutscene element
//cutsceneHandler.PlayNextElement();
// End the letterbox effect after zoom completes
//StartCoroutine(LetterboxEffect(false)); // Hide the letterbox effect