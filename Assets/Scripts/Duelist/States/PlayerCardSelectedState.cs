using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSelectedState : DuelistState {
    private Card selectedCard;

    public PlayerCardSelectedState(Card selectedCard) {
        this.selectedCard = selectedCard;
    }

    public override void EnterState() {
        // throw new System.NotImplementedException();
        // For now, use Hand as holder
        // highlightedHolder = duelist.Hand;
        // this.HighlightedCard = highlightedHolder.Cards[0];
        selectedCard.SetSelectedColor();
    }

    public override DuelistState HandleState() {
        Debug.Log("In player card selected state");

        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Revert to base state, keeping the original card
            return new PlayerBaseState(selectedCard);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            // Play the card (TODO: Add intermediate step if there is some requirement)
            duelist.PlaySelectedCard(selectedCard);
            selectedCard.ResetColor();

            return new EndState();
        }

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }
}
