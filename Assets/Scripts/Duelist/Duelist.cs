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

public class Duelist : MonoBehaviour {
    private Hand hand;
    private Deck deck;
    private Field field;

    private Dictionary<ResourceSO, int> currentResources;

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

        currentResources = new Dictionary<ResourceSO, int>();
    }

    public void DrawCard() {
        Card card = deck.DrawCard();

        if (card != null) {
            hand.AddCard(card);
        }

        OrganizeArea();
    }

    public void PlaySelectedCard(Card card) {
        int cardIndex = this.HandCards.IndexOf(card);
        if (cardIndex >= 0) {
            hand.RemoveCard(cardIndex);

            if (card.IsResourceCard()) {
                ResourceSO resource = card.GetResource();

                if (!currentResources.ContainsKey(resource)) {
                    currentResources.Add(resource, 0);
                }

                currentResources[resource] += card.Power;

                Debug.Log($"Adding power for resource {resource}, new count = {currentResources[resource]}");
            }
            field.AddCard(card);
        }

        OrganizeArea();
    }

    void OrganizeArea() {
        // This is called whenever a card based operation occurs

        // TODO: Since the spreading functions use a pretty similar formula, could try to extrapolate?
        // Field
        field.SpreadField();

        // Hand
        hand.SpreadHand();
    }
}
