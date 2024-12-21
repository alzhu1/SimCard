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
            // Card can always be previewed
            allowedActions = new List<PlayerCardAction> {
                PlayerCardAction.Preview
            };
            if (IsCardSummonAllowed()) {
                allowedActions.Add(PlayerCardAction.Summon);
            }

            actionIndex = 0;
            playerDuelist.CardGameManager.EventBus.OnPlayerCardSelect.Raise(new(selectedCard, allowedActions));
            playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(allowedActions[actionIndex]));
        }

        protected override void Exit() {
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
                            duelist.PlaySelectedCard(selectedCard);

                            if (selectedCard.NonSelfEffects.Count > 0) {
                                nextState = new PlayerCardEffectSelectionState(selectedCard);
                            } else {
                                nextState = new PlayerBaseState();
                            }
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

            return duelist.Currency >= selectedCard.Cost;
        }
    }
}
