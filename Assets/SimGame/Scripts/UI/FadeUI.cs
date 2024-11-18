using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour {
    private CanvasGroup fadeGroup;

    void Awake() {
        fadeGroup = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeInOut() {
        float t = 0;

        while (t < 1) {
            fadeGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
            t += Time.deltaTime;
        }
        fadeGroup.alpha = 1;

        yield return new WaitForSeconds(1f);

        t = 0;
        while (t < 1) {
            fadeGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
            t += Time.deltaTime;
        }
        fadeGroup.alpha = 0;
    }
}
