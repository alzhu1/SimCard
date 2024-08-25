using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSummonState : DuelistState {
    private Card cardToSummon;

    // Tribute summon
    private Dictionary<EntitySO, List<Card>> suppliedCards;
    private Card highlightedCard;
    private Card HighlightedCard {
        get { return highlightedCard; }
        set {
            highlightedCard?.ResetColor();
            value?.SetHighlightedColor();
            highlightedCard = value;
        }
    }

    private List<Card> holderCopy;

    public PlayerCardSummonState(Card cardToSummon) {
        this.cardToSummon = cardToSummon;
    }

    public override void EnterState() {
        // throw new System.NotImplementedException();
        // For now, use Hand as holder
        // highlightedHolder = duelist.Hand;
        // this.HighlightedCard = highlightedHolder.Cards[0];

        switch (cardToSummon.SummonType) {
            case CardSummonType.Tribute: {
                suppliedCards = new Dictionary<EntitySO, List<Card>>();
                foreach (var nonResourceCost in cardToSummon.NonResourceCosts) {
                    suppliedCards[nonResourceCost.entity] = new List<Card>();
                }

                this.HighlightedCard = duelist.Field.Cards?[0];
                holderCopy = new List<Card>(duelist.Field.Cards);
                break;
            }
        }
    }

    public override DuelistState HandleState() {
        Debug.Log("In player card summon state");

        if (Input.GetKeyDown(KeyCode.Escape)) {
            return new PlayerCardSelectedState(cardToSummon);
        }

        switch (cardToSummon.SummonType) {
            case CardSummonType.Regular:
                return HandleRegularSummon();

            case CardSummonType.Tribute:
                return HandleTributeSummon();
        }

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }

    DuelistState HandleRegularSummon() {
        // Logic goes here for regular summon stuff
        duelist.PlaySelectedCard(cardToSummon);
        cardToSummon.ResetColor();

        return new EndState();
    }

    DuelistState HandleTributeSummon() {
        // TODO: Take in input

        if (IsTributeSummonAllowed()) {
            this.HighlightedCard = null;

            if (Input.GetKeyDown(KeyCode.Space)) {
                duelist.PlaySelectedCard(cardToSummon, suppliedCards.Values);
                cardToSummon.ResetColor();
                return new EndState();
            }

            return null;
        }

        if (suppliedCards.ContainsKey(this.HighlightedCard.Entity) && Input.GetKeyDown(KeyCode.Space)) {
            // It is a cost we can add
            suppliedCards[this.HighlightedCard.Entity].Add(this.HighlightedCard);

            Card addedCard = this.HighlightedCard;
            holderCopy.Remove(addedCard);
            this.HighlightedCard = holderCopy.Count > 0 ? holderCopy[0] : null;
            addedCard.SetSelectedColor();
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

    bool IsTributeSummonAllowed() {
        foreach (var nonResourceCost in cardToSummon.NonResourceCosts) {
            if (suppliedCards[nonResourceCost.entity].Count != nonResourceCost.cost) {
                return false;
            }
        }

        Debug.Log("Allowed!");

        return true;
    }
}
