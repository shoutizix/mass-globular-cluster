using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInUI : MonoBehaviour
{
    public float fadeTime = 1;
    public float startDelay = 0;

    private CanvasGroup canvasGroup;
    private bool doneFading;

    private void OnEnable()
    {
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        Reset();
    }

    public void TriggerFadeIn()
    {
        StartCoroutine(FadeIn(fadeTime, startDelay));
    }

    public void TriggerReset(float delay = 0)
    {
        if (canvasGroup.alpha != 1)
        {
            Invoke(nameof(Reset), delay);
        } else
        {
            canvasGroup.alpha = 0;
        }
    }

    private void Reset()
    {
        StopAllCoroutines();
        canvasGroup.alpha = 0;
    }

    private IEnumerator FadeIn(float fadeTime, float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        float time = 0;
        float startAlpha = canvasGroup.alpha;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, time / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}
