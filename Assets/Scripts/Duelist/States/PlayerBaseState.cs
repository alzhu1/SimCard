using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : DuelistState {
    private CardHolder highlightedHolder;

    private Card highlightedCard;
    private Card HighlightedCard {
        get { return highlightedCard; }
        set {
            highlightedCard?.ResetColor();
            value?.SetHighlightedColor();
            highlightedCard = value;
        }
    }

    public PlayerBaseState() {}
    public PlayerBaseState(Card highlightedCard) {
        this.HighlightedCard = highlightedCard;
    }

    protected override void Enter() {
        // For now, use Hand as holder
        highlightedHolder = duelist.Hand;

        if (this.HighlightedCard == null) {
            this.HighlightedCard = highlightedHolder.Cards[0];
        }
    }

    protected override void Exit() {
        this.HighlightedCard = null;
    }

    protected override IEnumerator Handle() {
        Debug.Log("In player base state");

        // TODO: For now input can be put here (maybe that's ok?)
        // But think about where we could place player input and somehow merge that with DuelistController

        while (nextState == null) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                nextState = new EndState();
                break;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                // Move to a new state
                nextState = new PlayerCardSelectedState(this.HighlightedCard);
                break;
            }

            int nextCard = 0;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                nextCard = -1;
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                nextCard = 1;
            }

            if (nextCard != 0) {
                int nextCardIndex = highlightedHolder.Cards.IndexOf(this.HighlightedCard) + nextCard;
                int cardCount = highlightedHolder.Cards.Count;

                int highlightedCardIndex = nextCardIndex < 0 ? nextCardIndex + cardCount : nextCardIndex % cardCount;
                this.HighlightedCard = highlightedHolder.Cards[highlightedCardIndex];
            }

            yield return null;
        }
    }
}
