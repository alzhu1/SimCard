using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimCard.CardGame {
    public class CardGraph {
        private CardNode currNode;

        public Card CurrCard => currNode.card;

        // TODO: Graveyard - Field - Deck will be a row
        // However, the hover should treat graveyard and deck as one thing
        // Might need to add a "Card" attribute to the Graveyard or Deck
        public CardGraph(List<CardHolder> cardHoldersByRow, Card startingCard) {
            // First, iterate through all cards by row, replace with CardNode
            // Set left/right

            List<List<CardNode>> nodesByRow = cardHoldersByRow
                .Where(cardHolder => cardHolder.Cards.Count > 0)
                .Select(cardHolder => cardHolder.Cards.Select(card => new CardNode(card)).ToList())
                .ToList();
            int rowCount = nodesByRow.Count;

            for (int i = 0; i < rowCount; i++) {
                List<CardNode> row = nodesByRow[i];

                // Get prev/next row to set for up/down (should loop)
                List<CardNode> prevRow = nodesByRow[(rowCount + i - 1) % rowCount];
                List<CardNode> nextRow = nodesByRow[(i + 1) % rowCount];

                for (int j = 0; j < row.Count; j++) {
                    CardNode node = row[j];

                    if (node.card == startingCard) {
                        currNode = node;
                    }

                    node.Left = row[(row.Count + j - 1) % row.Count];
                    node.Right = row[(j + 1) % row.Count];

                    // For now, set the up/down to be the first item in the row
                    // nextRow should represent the row below, and prevRow goes up
                    node.Down = nextRow[0];
                    node.Up = prevRow[0];
                }
            }

            if (rowCount > 0 && currNode == null) {
                currNode = nodesByRow[0][0];
            }
        }

        public void MoveNode(Vector2Int direction) {
            currNode = currNode.dir[direction];
        }

        private sealed class CardNode {
            public Card card;

            public Dictionary<Vector2Int, CardNode> dir =
                new()
                {
                    { Vector2Int.up, null },
                    { Vector2Int.right, null },
                    { Vector2Int.down, null },
                    { Vector2Int.left, null },
                };

            public CardNode Up {
                get { return dir[Vector2Int.up]; }
                set { dir[Vector2Int.up] = value; }
            }

            public CardNode Right {
                get { return dir[Vector2Int.right]; }
                set { dir[Vector2Int.right] = value; }
            }

            public CardNode Down {
                get { return dir[Vector2Int.down]; }
                set { dir[Vector2Int.down] = value; }
            }

            public CardNode Left {
                get { return dir[Vector2Int.left]; }
                set { dir[Vector2Int.left] = value; }
            }

            public CardNode(Card card) {
                this.card = card;
            }
        }
    }
}
