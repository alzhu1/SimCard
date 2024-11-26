using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class PlayerBaseState : PlayerState {
        private readonly Card startingCard;

        private CardGraph cardGraph;

        public PlayerBaseState() { }

        public PlayerBaseState(Card startingCard) {
            this.startingCard = startingCard;
        }

        protected override void Enter() {
            // For now, use Hand as holder
            // Initialize list of cards

            cardGraph = new CardGraph(new() {
                playerDuelist.Hand.Cards
            }, startingCard);

            // Set cursor position
            playerDuelist.ShowCursor();
            playerDuelist.MoveCursorToCard(cardGraph.CurrCard, true);
            playerDuelist.CardGameManager.EventBus.OnPlayerCardHover.Raise(new(cardGraph.CurrCard, new()));
        }

        protected override void Exit() {
            cardGraph.Exit();
            playerDuelist.HideCursor();
        }

        protected override IEnumerator Handle() {
            // TODO: For now input can be put here (maybe that's ok?)
            // But think about where we could place player input and somehow merge that with DuelistController

            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    nextState = new EndState();
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    // Move to a new state
                    nextState = new PlayerCardSelectedState(cardGraph.CurrCard);
                    break;
                }

                Vector2Int move = Vector2Int.zero;

                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    move = Vector2Int.left;
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    move = Vector2Int.right;
                }

                if (!move.Equals(Vector2Int.zero)) {
                    // Card fromCard = cardGraph.CurrCard;
                    cardGraph.MoveNode(move);
                    Card toCard = cardGraph.CurrCard;

                    yield return playerDuelist.MoveCursorToCard(toCard);
                    playerDuelist.CardGameManager.EventBus.OnPlayerCardHover.Raise(new(cardGraph.CurrCard, new()));
                }

                yield return null;
            }
        }
    }
}
