using System.Collections;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    [SerializeField] private CanvasGroup Image;
    private float fadeOutDuration = 1.0f;

    private void OnEnable()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float currentTime = 0f;

        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            Image.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeOutDuration);
            yield return null;
        }
    }
}
