using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class PlayerBaseState : PlayerState {
        private readonly CardGraphSelectable startingCard;

        private CardGraph<CardGraphSelectable> cardGraph;

        public PlayerBaseState() { }

        public PlayerBaseState(CardGraphSelectable startingCard) {
            this.startingCard = startingCard;
        }

        protected override void Enter() {
            // Initialize list of cards
            CardGraphSelectable initItem = startingCard ?? playerDuelist.Hand.Cards[0];

            List<CardGraphSelectable> enemyPlayField = new();
            enemyPlayField.Add(playerDuelist.Enemy.Deck);
            enemyPlayField.AddRange(playerDuelist.Enemy.Field.Cards);
            enemyPlayField.Add(playerDuelist.Enemy.Graveyard);

            List<CardGraphSelectable> playerPlayField = new();
            playerPlayField.Add(playerDuelist.Graveyard);
            playerPlayField.AddRange(playerDuelist.Field.Cards);
            playerPlayField.Add(playerDuelist.Deck);

            cardGraph = new CardGraph<CardGraphSelectable>(new() {
                enemyPlayField,
                playerPlayField,
                new(playerDuelist.Hand.Cards)
            }, initItem);

            // Set cursor position
            playerDuelist.ShowCursor();
            playerDuelist.MoveCursorTo(cardGraph.CurrItem, true);
            playerDuelist.CardGameManager.EventBus.OnPlayerBaseHover.Raise(new(cardGraph.CurrItem));
        }

        protected override void Exit() {
            playerDuelist.HideCursor();
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    yield return FixHandSize();
                    nextState = new EndState();
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Space)) {
                    // Move to a new state
                    nextState = new PlayerCardSelectedState(cardGraph.CurrItem);
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
                    cardGraph.MoveNode(move);
                    yield return playerDuelist.MoveCursorTo(cardGraph.CurrItem);
                    playerDuelist.CardGameManager.EventBus.OnPlayerBaseHover.Raise(new(cardGraph.CurrItem));
                }

                yield return null;
            }
        }

        IEnumerator FixHandSize() {
            if (playerDuelist.Hand.Cards.Count <= Duelist.MAX_HAND_CARDS) {
                yield break;
            }

            CardGraph<Card> discardCardGraph = new CardGraph<Card>(new() {
                playerDuelist.Hand.Cards
            }, playerDuelist.Hand.Cards[0]);
            playerDuelist.CardGameManager.EventBus.OnPlayerBaseHover.Raise(new(discardCardGraph.CurrItem));

            yield return null;

            while (playerDuelist.Hand.Cards.Count > Duelist.MAX_HAND_CARDS) {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    // Discard the selected card
                    playerDuelist.FireCard(discardCardGraph.CurrItem);
                    discardCardGraph = new CardGraph<Card>(new() {
                        playerDuelist.Hand.Cards
                    }, playerDuelist.Hand.Cards[0]);
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
                    discardCardGraph.MoveNode(move);
                    yield return playerDuelist.MoveCursorTo(discardCardGraph.CurrItem);
                    playerDuelist.CardGameManager.EventBus.OnPlayerBaseHover.Raise(new(discardCardGraph.CurrItem));
                }

                yield return null;
            }
        }
    }
}
