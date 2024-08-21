using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSummonRequirementState : DuelistState {
    private Card selectedCard;

    private CardHolder highlightedHolder;
    private List<Card> holderCopy;

    private Card highlightedCard;
    private Card HighlightedCard {
        get { return highlightedCard; }
        set {
            highlightedCard?.ResetColor();
            value?.SetHighlightedColor();
            highlightedCard = value;
        }
    }

    private Dictionary<CardSO, List<Card>> suppliedCards;
    private Dictionary<ResourceSO, List<Card>> suppliedResources;

    public PlayerCardSummonRequirementState(Card selectedCard) {
        this.selectedCard = selectedCard;
    }

    public override void EnterState() {
        // throw new System.NotImplementedException();
        // For now, use Hand as holder
        // highlightedHolder = duelist.Hand;
        // this.HighlightedCard = highlightedHolder.Cards[0];
        selectedCard.SetSelectedColor();

        highlightedHolder = duelist.Field;
        this.HighlightedCard = highlightedHolder.Cards[0];

        // Copy list that can be modified
        holderCopy = new List<Card>(highlightedHolder.Cards);

        suppliedCards = new Dictionary<CardSO, List<Card>>();
        suppliedResources = new Dictionary<ResourceSO, List<Card>>();
    }

    public override DuelistState HandleState() {
        Debug.Log("In player card summon requirement state");

        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Revert to the previous state
            return new PlayerBaseState(selectedCard);
        }

        // First check if we meet all requirements
        if (ValidateCostsMet()) {
            // If so, another space press will play this card
            if (Input.GetKeyDown(KeyCode.Space)) {
                // Play the card
                // TODO: Need to remove the supplied cards
                duelist.PlaySelectedCard(selectedCard);
                selectedCard.ResetColor();

                return new EndState();
            }
        }

        if (this.HighlightedCard == null) {
            Debug.Log("Highlighted card is null, no op");
            return null;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            // You selected a card, only add if the card needs it
            if (this.HighlightedCard.IsResourceCard()) {
                Debug.Log("Is resource??");
                ResourceSO resource = this.HighlightedCard.GetResource();
                if (!suppliedResources.ContainsKey(resource)) {
                    suppliedResources.Add(resource, new List<Card>());
                }

                suppliedResources[resource].Add(this.HighlightedCard);
            
                Debug.Log($"Added {resource.name} to resource list");
            } else {
                Debug.Log("Is card??");
                CardSO card = this.HighlightedCard.CardSO;
                if (!suppliedCards.ContainsKey(card)) {
                    suppliedCards.Add(card, new List<Card>());
                }

                suppliedCards[card].Add(this.HighlightedCard);
            }

            Card summonedCard = this.HighlightedCard;

            // Remove from the copy
            holderCopy.Remove(this.HighlightedCard);
            this.HighlightedCard = holderCopy.Count > 0 ? holderCopy[0] : null;
            summonedCard.SetSummonColor();
        }

        int nextCard = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            nextCard = -1;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            nextCard = 1;
        }

        if (nextCard != 0) {
            int nextCardIndex = holderCopy.IndexOf(this.HighlightedCard) + nextCard;
            int cardCount = holderCopy.Count;

            int highlightedCardIndex = nextCardIndex < 0 ? nextCardIndex + cardCount : nextCardIndex % cardCount;
            this.HighlightedCard = holderCopy[highlightedCardIndex];
        }

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();

        this.HighlightedCard = null;

        foreach (var cards in suppliedCards.Values) {
            foreach (var card in cards) {
                card.ResetColor();
            }
        }

        foreach (var cards in suppliedResources.Values) {
            foreach (var card in cards) {
                card.ResetColor();
            }
        }
    }

    bool ValidateCostsMet() {
        foreach (var cardCost in selectedCard.CardCosts) {
            if (!suppliedCards.ContainsKey(cardCost.card)) {
                return false;
            }

            if (suppliedCards[cardCost.card].Count != cardCost.cost) {
                return false;
            }
        }

        foreach (var resourceCost in selectedCard.ResourceCosts) {
            Debug.Log($"Current resource: {resourceCost.resource.name}: {resourceCost.cost}");
            if (!suppliedResources.ContainsKey(resourceCost.resource)) {
                return false;
            }

            Debug.Log("Going to check count");

            if (suppliedResources[resourceCost.resource].Count != resourceCost.cost) {
                return false;
            }
        }

        this.HighlightedCard = null;
        return true;
    }
}
