using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public enum PlayerCardAction {
        None,
        Preview,
        Summon,
        Fire
    }

    public class PlayerCardSelectedState : PlayerState {
        private readonly CardGraphSelectable selectedItem;

        private List<PlayerCardAction> allowedActions;
        private int actionIndex;

        public PlayerCardSelectedState(CardGraphSelectable selectedItem) {
            this.selectedItem = selectedItem;
        }

        protected override void Enter() {
            // Card can always be previewed
            allowedActions = new List<PlayerCardAction> {
                PlayerCardAction.Preview
            };

            if (selectedItem is Card) {
                Card selectedCard = selectedItem as Card;
                if (playerDuelist.IsCardSummonAllowed(selectedCard)) {
                    allowedActions.Add(PlayerCardAction.Summon);
                }

                if (playerDuelist.Field == selectedCard.GetCurrentHolder()) {
                    allowedActions.Add(PlayerCardAction.Fire);
                }
            }

            actionIndex = 0;
            playerDuelist.CardGameManager.EventBus.OnPlayerCardSelect.Raise(new(selectedItem, allowedActions));
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
                    nextState = new PlayerBaseState(selectedItem);
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
                            nextState = new PlayerCardPreviewState(selectedItem);
                            break;

                        case PlayerCardAction.Summon: {
                            Card selectedCard = selectedItem as Card;
                            duelist.PlaySelectedCard(selectedCard);

                            if (selectedCard.NonSelfEffects.Count > 0) {
                                nextState = new PlayerCardEffectSelectionState(selectedCard);
                            } else {
                                nextState = new PlayerBaseState();
                            }
                            break;
                        }

                        case PlayerCardAction.Fire: {
                            Card selectedCard = selectedItem as Card;
                            selectedCard.GetCurrentHolder().TransferTo(playerDuelist.Graveyard, selectedCard, false);
                            nextState = new PlayerBaseState();
                            break;
                        }
                    }
                }

                yield return null;
            }
        }
    }
}
