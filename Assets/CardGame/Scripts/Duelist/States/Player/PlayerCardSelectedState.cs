using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public enum PlayerCardAction {
        None,
        Preview,
        Summon
    }

    public class PlayerCardSelectedState : PlayerState {
        private readonly Card selectedCard;

        private List<PlayerCardAction> allowedActions;
        private int actionIndex;

        public PlayerCardSelectedState(Card selectedCard) {
            this.selectedCard = selectedCard;
        }

        protected override void Enter() {
            selectedCard.SetSelectedColor();

            // Card can always be previewed
            allowedActions = new List<PlayerCardAction> {
                PlayerCardAction.Preview
            };
            if (IsCardSummonAllowed()) {
                allowedActions.Add(PlayerCardAction.Summon);
            }

            playerDuelist.CardGameManager.EventBus.OnPlayerCardSelect.Raise(new(selectedCard, allowedActions));

            actionIndex = 0;
            playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(allowedActions[actionIndex]));
        }

        protected override void Exit() {
            selectedCard.ResetColor();
            playerDuelist.CardGameManager.EventBus.OnPlayerCardSelect.Raise(new(null, new()));
            playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(PlayerCardAction.None));
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    // Revert to base state, keeping the original card
                    nextState = new PlayerBaseState(selectedCard);
                    break;
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    actionIndex = (allowedActions.Count + actionIndex - 1) % allowedActions.Count;
                    playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(allowedActions[actionIndex]));
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    actionIndex = (actionIndex + 1) % allowedActions.Count;
                    playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(allowedActions[actionIndex]));
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    switch (allowedActions[actionIndex]) {
                        case PlayerCardAction.Preview:
                            nextState = new PlayerCardPreviewState(selectedCard);
                            break;

                        case PlayerCardAction.Summon:
                            nextState = new PlayerCardSummonState(selectedCard);
                            break;
                    }
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
                if (duelist.CurrentResources[resourceCost.entity] < resourceCost.cost) {
                    return false;
                }
            }

            foreach (var nonResourceCost in selectedCard.NonResourceCosts) {
                if (!duelist.Field.HasEntityCount(nonResourceCost.entity, nonResourceCost.cost)) {
                    return false;
                }
            }

            return true;
        }
    }
}
