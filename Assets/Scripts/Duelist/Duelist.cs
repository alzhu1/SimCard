using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Thinking that this class should contain a state machine in it
// State machine to determine the current action being taken
// Enter on DRAW
// DRAW -> BASE
// BASE phase can go to multiple options; CARD_SELECTED, PREVIEW_CARD, VIEW_DECK, etc
// There are some actions that seem like an AI wouldn't need to do, but if we model it as such anyways,
// it might make the AI programming a little easier? Perhaps enemy AI can have its own state machine

public enum DuelistType {
    HUMAN,
    AI
}

public class Duelist : MonoBehaviour {
    [HideInInspector] public DuelistType type;

    private Hand hand;
    private Deck deck;
    private Field field;

    public Hand Hand => hand;

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

        if (card != null) {
            hand.AddCard(card);
        }
    }

    public void PlaySelectedCard(Card card) {
        int cardIndex = this.HandCards.IndexOf(card);
        if (cardIndex >= 0) {
            hand.RemoveCard(cardIndex);
            field.AddCard(card);
        }
    }
}
