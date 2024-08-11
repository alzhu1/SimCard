using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : DuelistState {
    private CardHolder highlightedHolder;

    private Card _highlightedCard;
    private Card HighlightedCard {
        get { return _highlightedCard; }
        set {
            _highlightedCard?.ResetSelectedColor();
            value?.SetSelectedColor();
            _highlightedCard = value;
        }
    }

    public override void EnterState() {
        // throw new System.NotImplementedException();
        // For now, use Hand as holder
        highlightedHolder = duelist.Hand;
        this.HighlightedCard = highlightedHolder.Cards[0];
    }

    public override DuelistState HandleState() {
        Debug.Log("In player base state");

        // TODO: For now input can be put here (maybe that's ok?)
        // But think about where we could place player input and somehow merge that with DuelistController

        if (Input.GetKeyDown(KeyCode.Space)) {
            // TODO: This logic is for playing the card, move to a different state
            duelist.PlaySelectedCard(this.HighlightedCard);
            this.HighlightedCard = null;

            // Move to a new state
            return new EndState();
        }

        int nextCard = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            nextCard = -1;
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            nextCard = 1;
        }

        if (nextCard != 0) {
            int cardIndex = highlightedHolder.Cards.IndexOf(this.HighlightedCard);
            this.HighlightedCard = highlightedHolder.Cards[(cardIndex + nextCard) % highlightedHolder.Cards.Count];
        }

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }
}
