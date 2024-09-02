using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : CardHolder {
    private SpriteRenderer deckSr;

    public Card NextCard => cards.Count > 0 ? cards[0] : null;

    protected override void Awake() {
        base.Awake();
        deckSr = GetComponent<SpriteRenderer>();
    }

    public override void Spread() { }

    public void TryHideDeck() {
        if (cards.Count == 0) {
            deckSr.enabled = false;
        }
    }
}
