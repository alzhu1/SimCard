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
                playerDuelist.Enemy.Field,
                playerDuelist.Field
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
                        playerDuelist.Enemy.Field,
                        playerDuelist.Field
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
                    cardEffectSelectionGraph.MoveNode(move);
                    Card toCard = cardEffectSelectionGraph.CurrCard;

                    yield return playerDuelist.MoveCursorToCard(toCard);
                    playerDuelist.CardGameManager.EventBus.OnPlayerCardHover.Raise(new(cardEffectSelectionGraph.CurrCard, new()));
                }

                yield return null;
            }

            nextState = new PlayerBaseState();
        }
    }
}
