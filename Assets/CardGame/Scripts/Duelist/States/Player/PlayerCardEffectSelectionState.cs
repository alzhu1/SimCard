using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class PlayerCardEffectSelectionState : PlayerState {
        private readonly Card playedCard;
        private int nonSelfEffectIndex;

        private CardGraph<Card> cardEffectSelectionGraph;

        public PlayerCardEffectSelectionState(Card playedCard) {
            this.playedCard = playedCard;
            nonSelfEffectIndex = 0;
        }

        protected override void Enter() {
            cardEffectSelectionGraph = new CardGraph<Card>(new() {
                playerDuelist.Enemy.Field.Cards,
                playerDuelist.Field.Cards
            }, playedCard);

            // Set cursor position
            playerDuelist.ShowCursor();
            playerDuelist.MoveCursorTo(cardEffectSelectionGraph.CurrItem, true);
            playerDuelist.CardGameManager.EventBus.OnPlayerCardEffectHover.Raise(new(cardEffectSelectionGraph.CurrItem, playedCard.NonSelfEffects[nonSelfEffectIndex]));
        }

        protected override void Exit() {
            playerDuelist.HideCursor();
        }

        protected override IEnumerator Handle() {
            while (nonSelfEffectIndex < playedCard.NonSelfEffects.Count) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    Effect effect = playedCard.NonSelfEffects[nonSelfEffectIndex];
                    playerDuelist.ApplyCardEffect(effect, playedCard, cardEffectSelectionGraph.CurrItem);

                    nonSelfEffectIndex++;

                    Debug.Log("Effect has been applied");
                    cardEffectSelectionGraph = new CardGraph<Card>(new() {
                        playerDuelist.Enemy.Field.Cards,
                        playerDuelist.Field.Cards
                    }, playedCard);
                    playerDuelist.MoveCursorTo(cardEffectSelectionGraph.CurrItem, true);

                    if (nonSelfEffectIndex < playedCard.NonSelfEffects.Count) {
                        playerDuelist.CardGameManager.EventBus.OnPlayerCardEffectHover.Raise(new(cardEffectSelectionGraph.CurrItem, playedCard.NonSelfEffects[nonSelfEffectIndex]));
                    }

                    yield return null;
                    continue;
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
                    yield return playerDuelist.MoveCursorTo(cardEffectSelectionGraph.CurrItem);
                    playerDuelist.CardGameManager.EventBus.OnPlayerCardEffectHover.Raise(new(cardEffectSelectionGraph.CurrItem, playedCard.NonSelfEffects[nonSelfEffectIndex]));
                }

                yield return null;
            }

            nextState = new PlayerBaseState();
        }
    }
}
