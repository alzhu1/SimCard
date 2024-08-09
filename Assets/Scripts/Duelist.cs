using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duelist : MonoBehaviour {
    private Hand hand;
    private Deck deck;

    void Awake() {
        hand = GetComponentInChildren<Hand>();
        deck = GetComponentInChildren<Deck>();
    }

    public Hand GetHand() {
        return hand;
    }

    public Deck GetDeck() {
        return deck;
    }

    public void DrawCard() {
        Card card = deck.DrawCard();
        hand.AddCard(card);
    }

    // TODO: Generalize this with input
    public void PlayFirstCard() {
        hand.RemoveCard(0);
    }
}
