using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
    // TODO: Init this in editor?
    // But later, should have some kind of Init function elsewhere
    // Init function should take CardSO -> Card
    public List<Card> cards;

    private Duelist duelist;

    void Awake() {
        duelist = GetComponentInParent<Duelist>();
    }

    void Update() {
        
    }

    public Card DrawCard() {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}
