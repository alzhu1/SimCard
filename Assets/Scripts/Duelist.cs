using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duelist : MonoBehaviour {
    private Hand hand;
    private Deck deck;
    private Field field;

    public List<Card> HandCards {
        get { return hand.Cards; }
    }

    public List<Card> DeckCards {
        get { return deck.Cards; }
    }

    public List<Card> FieldCards {
        get { return field.Cards; }
    }

    void Awake() {
        hand = GetComponentInChildren<Hand>();
        deck = GetComponentInChildren<Deck>();
        field = GetComponentInChildren<Field>();
    }

    public void DrawCard() {
        Card card = deck.DrawCard();
        hand.AddCard(card);
    }

    public void PlaySelectedCard(Card card) {
        int cardIndex = this.HandCards.IndexOf(card);
        if (cardIndex >= 0) {
            hand.RemoveCard(cardIndex);
            field.AddCard(card);
        }
    }
}
