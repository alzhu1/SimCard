using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardSelectedState : PlayerState {
    private readonly Card selectedCard;
    private bool isSummonAllowed;

    public PlayerCardSelectedState(Card selectedCard) {
        this.selectedCard = selectedCard;
    }

    protected override void Enter() {
        selectedCard.SetSelectedColor();

        // TODO: Depending on some criteria, we should determine a "strategy" to execute
        // i.e. different functions to handle different cases?
        isSummonAllowed = IsCardSummonAllowed();
    }

    protected override void Exit() {
        selectedCard.ResetColor();
    }

    protected override IEnumerator Handle() {

        while (nextState == null) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                // Revert to base state, keeping the original card
                nextState = new PlayerBaseState(selectedCard);
                break;
            }

            if (isSummonAllowed && Input.GetKeyDown(KeyCode.Space)) {
                nextState = new PlayerCardSummonState(selectedCard);
            }

            yield return null;
        }
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
