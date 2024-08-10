using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    [SerializeField] private CardSO cardSO;

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

    public void PlayCard(List<Card> sacrifices) {}
    public void SacrificeCard() {}

    public void SetSelectedColor() {
        sr.color = Color.yellow;
    }
    public void ResetSelectedColor() {
        Debug.Log("Resetting color");
        sr.color = Color.white;
    }
}
