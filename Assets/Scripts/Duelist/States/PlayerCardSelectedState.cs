using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSelectedState : DuelistState {
    private Card selectedCard;
    private bool isSummonAllowed;

    public PlayerCardSelectedState(Card selectedCard) {
        this.selectedCard = selectedCard;
    }

    public override void EnterState() {
        // throw new System.NotImplementedException();
        // For now, use Hand as holder
        // highlightedHolder = duelist.Hand;
        // this.HighlightedCard = highlightedHolder.Cards[0];
        selectedCard.SetSelectedColor();

        // TODO: Depending on some criteria, we should determine a "strategy" to execute
        // i.e. different functions to handle different cases?
        isSummonAllowed = IsCardSummonAllowed();
    }

    public override DuelistState HandleState() {
        Debug.Log("In player card selected state");

        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Revert to base state, keeping the original card
            return new PlayerBaseState(selectedCard);
        }

        // TODO: Here, we should check if the card can even be summoned

        // TODO: Have better way to separate logic
        // if (selectedCard.HasNonResourceCosts()) {
        //     return HandleTributeSummon();
        // } else {
        //     return HandleRegularSummon();
        // }
        if (!isSummonAllowed) {
            return null;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            return new PlayerCardSummonState(selectedCard);
        }

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }

    bool IsCardSummonAllowed() {
        // Card summon is allowed if resource cost is met
        // And other cards exist on the field

        foreach (var resourceCost in selectedCard.ResourceCosts) {
            ResourceEntitySO resource = resourceCost.entity;

            if (duelist.CurrentResources[resource] < resourceCost.cost) {
                return false;
            }
        }

        foreach (var nonResourceCost in selectedCard.NonResourceCosts) {
            EntitySO entity = nonResourceCost.entity;

            if (!duelist.Field.HasEntityCount(entity, nonResourceCost.cost)) {
                return false;
            }
        }

        return true;
    }
}
