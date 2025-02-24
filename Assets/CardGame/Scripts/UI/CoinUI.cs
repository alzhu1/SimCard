using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour {
    [SerializeField] private Sprite headsSprite;
    [SerializeField] private Sprite tailsSprite;

    [SerializeField] private float coinBounceHeight = 1f;
    [SerializeField] private float bounceTime = 1f;
    [SerializeField] private float resultWaitTime = 1f;
    [SerializeField] private float fadeTime = 1f;

    private Image coinImage;
    private Animator animator;

    void Awake() {
        coinImage = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    void Update() {

    }

    public IEnumerator FlipCoin(float value) {
        animator.enabled = false;
        Sprite coinSprite = value < 0.5f ? headsSprite : tailsSprite;
        coinImage.sprite = coinSprite;

        float baseHeight = coinImage.rectTransform.anchoredPosition.y;

        // First value of t that returns 1 from EaseOutBounce
        float t = -bounceTime / 2.75f;
        while (t < bounceTime) {
            coinImage.rectTransform.anchoredPosition = new Vector2(
                coinImage.rectTransform.anchoredPosition.x,
                baseHeight + ((1 - EaseOutBounce(t / bounceTime)) * coinBounceHeight)
            );
            yield return null;
            t += Time.deltaTime;
        }
        coinImage.rectTransform.anchoredPosition = new Vector2(
            coinImage.rectTransform.anchoredPosition.x,
            baseHeight
        );

        yield return new WaitForSeconds(resultWaitTime);

        t = 0;
        Color whiteClear = new Color(1, 1, 1, 0);
        while (t < fadeTime) {
            coinImage.color = Color.Lerp(Color.white, whiteClear, t / fadeTime);
            yield return null;
            t += Time.deltaTime;
        }
        coinImage.color = Color.clear;

        Debug.Log("Done");
    }

    // https://easings.net/
    float EaseOutBounce(float x) {
        // Allow to work with negative (basically flip across y axis)
        if (x < 0) {
            x = -x;
        }

        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1) {
            return n1 * x * x;
        } else if (x < 2 / d1) {
            x -= 1.5f / d1;
            return n1 * x * x + 0.75f;
        } else if (x < 2.5 / d1) {
            x -= 2.25f / d1;
            return n1 * x * x + 0.9375f;
        } else {
            x -= 2.625f / d1;
            return n1 * x * x + 0.984375f;
        }
    }
}
