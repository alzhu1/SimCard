using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Duelist : MonoBehaviour {
    private Hand hand;
    private Deck deck;
    private Field field;
    private Graveyard graveyard;

    private Dictionary<ResourceEntitySO, int> currentResources;

    public Hand Hand => hand;
    public Field Field => field;

    public Dictionary<ResourceEntitySO, int> CurrentResources => currentResources;

    public List<Card> HandCards {
        get { return hand.Cards; }
    }

    public List<Card> DeckCards {
        get { return deck.Cards; }
    }

    public List<Card> FieldCards {
        get { return field.Cards; }
    }

    public int TotalPower => Field?.Cards?.Sum(x => x.Power) ?? 0;

    void Awake() {
        hand = GetComponentInChildren<Hand>();
        deck = GetComponentInChildren<Deck>();
        field = GetComponentInChildren<Field>();
        graveyard = GetComponentInChildren<Graveyard>();

        currentResources = new Dictionary<ResourceEntitySO, int>();
    }

    // void Update() {
    //     // TODO: Remove this update function
    //     foreach (var resources in currentResources) {
    //         Debug.Log($"[Duelist] Resource {resources.Key.entityName} => {resources.Value}");
    //     }
    // }

    public void DrawCard() {
        Card card = deck.GetFirstCard();

        if (card != null) {
            deck.TransferTo(hand, card, true);
            deck.TryHideDeck();
        }

        OrganizeArea();
    }

    public void PlaySelectedCard(Card card, IEnumerable<List<Card>> cardSacrifices = null) {
        int cardIndex = this.HandCards.IndexOf(card);
        if (cardIndex >= 0) {
            // hand.RemoveCard(cardIndex);

            // Remove from resources
            foreach (var resourceCost in card.ResourceCosts) {
                currentResources[resourceCost.entity] -= resourceCost.cost;
            }

            if (card.IsResourceCard()) {
                ResourceEntitySO resource = card.GetResource();

                if (!currentResources.ContainsKey(resource)) {
                    currentResources.Add(resource, 0);
                }

                currentResources[resource] += card.Power;

                Debug.Log($"Adding power for resource {resource}, new count = {currentResources[resource]}");
            
                // graveyard.AddCard(card);
                hand.TransferTo(graveyard, card, false);
            } else {
                // field.AddCard(card);
                hand.TransferTo(field, card, true);
            }
        }

        if (cardSacrifices != null) {
            foreach (var cardList in cardSacrifices) {
                foreach (var cardSacrifice in cardList) {
                    field.TransferTo(graveyard, cardSacrifice, false);
                }
            }
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
