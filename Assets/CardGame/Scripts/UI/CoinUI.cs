using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUI : MonoBehaviour {
    private Image coinImage;
    private Animator animator;

    void Awake() {
        coinImage = GetComponent<Image>();
        animator = GetComponent<Animator>();
    }

    void Update() {

    }

    public IEnumerator FlipCoin(float value) {
        string trigger = value < 0.5f ? "Heads" : "Tails";
        animator.SetTrigger(trigger);
        yield return new WaitForSeconds(1f);

        float t = 0;
        Color whiteClear = new Color(1, 1, 1, 0);
        while (t < 1) {
            coinImage.color = Color.Lerp(Color.white, whiteClear, t);
            yield return null;
            t += Time.deltaTime;
        }
        coinImage.color = Color.clear;

        Debug.Log("Done");
    }
}
