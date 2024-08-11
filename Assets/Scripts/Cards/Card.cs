using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] private CardSO cardSO;

    public int Power => cardSO.power;

    // TODO: Card likely needs to carry info about its own state
    // i.e. owned by player/opponent, whether it's on the field, etc

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

    public void PlayCard() {}
    public void SacrificeCard() {}

    public bool IsResourceCard() {
        return cardSO.CardType == CardType.Resource;
    }

    public ResourceSO GetResource() {
        if (IsResourceCard()) {
            return ((ResourceCardSO)cardSO).resource;
        }
        return null;
    }

    public void SetHighlightedColor() {
        sr.color = Color.yellow;
    }

    public void SetSelectedColor() {
        sr.color = Color.green;
    }
    public void ResetColor() {
        Debug.Log("Resetting color");
        sr.color = Color.white;
    }
}
