using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour {
    [SerializeField] private Sprite headsSprite;
    [SerializeField] private Sprite tailsSprite;

    [SerializeField] private Sprite[] coinSprites;

    [SerializeField] private float animationFrameTime;

    [SerializeField] private float coinBounceHeight = 1f;
    [SerializeField] private float bounceTime = 1f;
    [SerializeField] private float resultWaitTime = 1f;
    [SerializeField] private float fadeTime = 1f;

    private Image coinImage;

    private Sprite targetSprite;
    private int spriteIndex;

    void Awake() {
        coinImage = GetComponent<Image>();
        spriteIndex = 0;
    }

    void Start() {
        StartCoroutine(Animate());
    }

    IEnumerator Animate() {
        while (true) {
            coinImage.sprite = coinSprites[spriteIndex];

            if (coinImage.sprite == targetSprite) {
                break;
            }

            yield return new WaitForSeconds(animationFrameTime);

            spriteIndex = (coinSprites.Length + spriteIndex + 1) % coinSprites.Length;
        }
    }

    public IEnumerator FlipCoin(float value, System.Action PlaySFX) {
        // animator.enabled = false;
        float baseHeight = coinImage.rectTransform.anchoredPosition.y;

        PlaySFX();

        // First value of t that returns 1 from EaseOutBounce
        float t = -bounceTime / 2.75f;
        float startEasing = EaseOutBounce(t / bounceTime);
        float[] easings = new float[] { startEasing, startEasing, startEasing };

        while (t < bounceTime) {
            // Move easings over by 1
            easings[0] = easings[1];
            easings[1] = easings[2];

            // float easing = EaseOutBounce(t / bounceTime);
            float heightDiff = (1 - easings[1]) * coinBounceHeight;

            coinImage.rectTransform.anchoredPosition = new Vector2(
                coinImage.rectTransform.anchoredPosition.x,
                baseHeight + heightDiff
            );
            yield return null;
            t += Time.deltaTime;

            // Recalculate the easing to see the next value
            easings[2] = EaseOutBounce(t / bounceTime);

            if (easings[0] < easings[1] && easings[2] < easings[1]) {
                PlaySFX();
            }

            // Pick a target sprite during flipping so that once it lands, it should be close to target
            // Not super accurate but works
            if (targetSprite == null && t >= bounceTime / 2) {
                targetSprite = value < 0.5f ? headsSprite : tailsSprite;
            }
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
