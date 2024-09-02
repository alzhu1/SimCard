using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardGraph {
    private CardNode _currNode;
    private CardNode CurrNode {
        get { return _currNode; }
        set {
            _currNode?.card?.ResetColor();
            value?.card?.SetHighlightedColor();
            _currNode = value;
        }
    }

    public Card CurrCard => CurrNode.card;

    // TODO: Graveyard - Field - Deck will be a row
    // However, the hover should treat graveyard and deck as one thing
    // Might need to add a "Card" attribute to the Graveyard or Deck
    public CardGraph(List<List<Card>> cardsByRow, Card startingCard) {
        // First, iterate through all cards by row, replace with CardNode
        // Set left/right

        List<List<CardNode>> nodesByRow = cardsByRow
            .Where(row => row.Count > 0)
            .Select(row => row.Select(card => new CardNode(card)).ToList())
            .ToList();
        int rowCount = nodesByRow.Count;

        for (int i = 0; i < rowCount; i++) {
            List<CardNode> row = nodesByRow[i];

            // Get prev/next row to set for up/down (should loop)
            List<CardNode> prevRow = nodesByRow[(rowCount + i - 1) % rowCount];
            List<CardNode> nextRow = nodesByRow[(i + 1) % rowCount];

            for (int j = 0; j < row.Count; j++) {
                CardNode currNode = row[j];

                if (currNode.card == startingCard) {
                    this.CurrNode = currNode;
                }

                currNode.Left = row[(row.Count + j - 1) % row.Count];
                currNode.Right = row[(j + 1) % row.Count];

                // For now, set the up/down to be the first item in the row
                currNode.Up = nextRow[0];
                currNode.Down = prevRow[0];
            }
        }

        if (rowCount > 0 && this.CurrNode == null) {
            this.CurrNode = nodesByRow[0][0];
        }
    }

    public void MoveNode(Vector2Int direction) {
        this.CurrNode = this.CurrNode.dir[direction];
    }

    public void Exit() {
        // Reset color
        this.CurrNode = null;
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
