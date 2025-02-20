using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour {
    private CanvasGroup fadeGroup;

    void Awake() {
        fadeGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeIn(float fadeTime = 1f) {
        float t = 0;

        while (t < fadeTime) {
            fadeGroup.alpha = Mathf.Lerp(0, 1, t / fadeTime);
            yield return null;
            t += Time.deltaTime;
        }
        fadeGroup.alpha = 1;
    }

    public IEnumerator FadeOut(float fadeTime = 1f) {
        float t = 0;
        while (t < fadeTime) {
            fadeGroup.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
            t += Time.deltaTime;
        }
        fadeGroup.alpha = 0;
    }

    public IEnumerator FadeInOut(float fadeInTime = 1f, float fadeOutTime = 1f) {
        yield return StartCoroutine(FadeIn(fadeInTime));

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeOut(fadeOutTime));
    }
}
