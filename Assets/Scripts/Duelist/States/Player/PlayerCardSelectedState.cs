using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class PlayerCardSelectedState : PlayerState {
        private readonly Card selectedCard;
        private bool isSummonAllowed;

        // TODO: Don't reference "icon" in name
        private string iconName;

        public PlayerCardSelectedState(Card selectedCard) {
            this.selectedCard = selectedCard;
        }

        protected override void Enter() {
            selectedCard.SetSelectedColor();

            // TODO: Depending on some criteria, we should determine a "strategy" to execute
            // i.e. different functions to handle different cases?
            isSummonAllowed = IsCardSummonAllowed();

            playerDuelist.CardGameManager.EventBus.OnPlayerCardSelect.Raise(new CardArgs(selectedCard).WithSummonAllowed(isSummonAllowed));

            iconName = "Preview";
            playerDuelist.CardGameManager.EventBus.OnCardIconHover.Raise(new IconArgs(iconName));
        }

        protected override void Exit() {
            selectedCard.ResetColor();
            playerDuelist.CardGameManager.EventBus.OnPlayerCardSelect.Raise(new CardArgs(null));
            playerDuelist.CardGameManager.EventBus.OnCardIconHover.Raise(new IconArgs(null));
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    // Revert to base state, keeping the original card
                    nextState = new PlayerBaseState(selectedCard);
                    break;
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
                    iconName = iconName.Equals("Preview") ? "Summon" : "Preview";
                    playerDuelist.CardGameManager.EventBus.OnCardIconHover.Raise(new IconArgs(iconName));
                }

                if (isSummonAllowed && Input.GetKeyDown(KeyCode.Space)) {
                    nextState = new PlayerCardSummonState(selectedCard);
                }

                yield return null;
            }
        }

        bool IsCardSummonAllowed() {
            // Duelist must have enough actions
            if (!duelist.AllowAction) {
                return false;
            }

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
}
