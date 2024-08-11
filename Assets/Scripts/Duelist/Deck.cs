using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : CardHolder {
    // TODO: Init this in editor?
    // But later, should have some kind of Init function elsewhere
    // Init function should take CardSO -> Card

    void Update() {
        
    }

    public Card DrawCard() {
        if (cards.Count > 0) {
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }

        return null;
    }
}
