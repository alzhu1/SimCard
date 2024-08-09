using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] private CardSO cardSO;

    private SpriteRenderer sr;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = cardSO.sprite;
        gameObject.SetActive(false);
    }

    void Start() {
        
    }

    void Update() {
        
    }

    public void PlayCard(List<Card> sacrifices) {}
    public void SacrificeCard() {}
}
