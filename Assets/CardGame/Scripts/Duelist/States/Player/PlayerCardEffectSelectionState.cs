using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class PlayerCardEffectSelectionState : PlayerState {
        private readonly Card playedCard;

        private CardGraph cardEffectSelectionGraph;

        public PlayerCardEffectSelectionState(Card playedCard) {
            this.playedCard = playedCard;
        }

        protected override void Enter() {
            cardEffectSelectionGraph = new CardGraph(new() {
                playerDuelist.Enemy.Field.Cards,
                playerDuelist.Field.Cards
            }, playedCard);

            // Set cursor position
            playerDuelist.ShowCursor();
            playerDuelist.MoveCursorToCard(cardEffectSelectionGraph.CurrCard, true);
            playerDuelist.CardGameManager.EventBus.OnPlayerCardHover.Raise(new(cardEffectSelectionGraph.CurrCard, new()));
        }

        protected override void Exit() {
            playerDuelist.HideCursor();
        }

        protected override IEnumerator Handle() {
            // TODO: Need to convey that the effect is being applied somewhere in UI
            int nonSelfEffectIndex = 0;
            while (nonSelfEffectIndex < playedCard.NonSelfEffects.Count) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Effect effect = playedCard.NonSelfEffects[nonSelfEffectIndex];
                    playerDuelist.ApplyCardEffect(effect, playedCard, cardEffectSelectionGraph.CurrCard);

                    nonSelfEffectIndex++;

                    Debug.Log("Effect has been applied");
                    cardEffectSelectionGraph = new CardGraph(new() {
                        playerDuelist.Enemy.Field.Cards,
                        playerDuelist.Field.Cards
                    }, playedCard);
                    playerDuelist.MoveCursorToCard(cardEffectSelectionGraph.CurrCard, true);
                    playerDuelist.CardGameManager.EventBus.OnPlayerCardHover.Raise(new(cardEffectSelectionGraph.CurrCard, new()));

                    break;
                }

                Vector2Int move = Vector2Int.zero;

                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    move = Vector2Int.left;
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    move = Vector2Int.right;
                } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    move = Vector2Int.up;
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    move = Vector2Int.down;
                }

                if (!move.Equals(Vector2Int.zero)) {
                    // Card fromCard = cardGraph.CurrCard;
                    cardEffectSelectionGraph.MoveNode(move);
                    Card toCard = cardEffectSelectionGraph.CurrCard;

                    yield return playerDuelist.MoveCursorToCard(toCard);
                    playerDuelist.CardGameManager.EventBus.OnPlayerCardHover.Raise(new(cardEffectSelectionGraph.CurrCard, new()));
                }

                yield return null;
            }

            nextState = new PlayerBaseState();

            // while (nextState == null) {
            //     if (Input.GetKeyDown(KeyCode.Escape)) {
            //         // Revert to base state, keeping the original card
            //         nextState = new PlayerBaseState(selectedCard);
            //         break;
            //     }

            //     if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            //         actionIndex = (allowedActions.Count + actionIndex - 1) % allowedActions.Count;
            //         playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(allowedActions[actionIndex]));
            //     } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            //         actionIndex = (actionIndex + 1) % allowedActions.Count;
            //         playerDuelist.CardGameManager.EventBus.OnCardActionHover.Raise(new(allowedActions[actionIndex]));
            //     }

            //     if (Input.GetKeyDown(KeyCode.Space)) {
            //         switch (allowedActions[actionIndex]) {
            //             case PlayerCardAction.Preview:
            //                 nextState = new PlayerCardPreviewState(selectedCard);
            //                 break;

            //             case PlayerCardAction.Summon:
            //                 // nextState = new PlayerCardSummonState(selectedCard);
            //                 duelist.PlaySelectedCard(selectedCard);

            //                 if (selectedCard.NonSelfEffects.Count > 0) {
            //                     // TODO: Create a new player state that will build a graph of cards on the field, for selecting which to apply effects on
            //                     // Only need to do this if list is not empty
            //                 }

            //                 nextState = new PlayerBaseState();
            //                 break;
            //         }
            //     }

            //     yield return null;
            // }
        }
    }
}
