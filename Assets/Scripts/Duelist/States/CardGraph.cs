using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardGraph {
    private CardNode currNode;

    public CardGraph(List<List<Card>> cardsByRow, Card startingCard) {
        // First, iterate through all cards by row, replace with CardNode
        // Set left/right

        List<List<CardNode>> nodesByRow = cardsByRow
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
                    this.currNode = currNode;
                }

                currNode.left = row[(row.Count + j - 1) % row.Count];
                currNode.right = row[(j + 1) % row.Count];

                // For now, set the up/down to be the first item in the row
                currNode.up = nextRow[0];
                currNode.down = prevRow[0];
            }
        }
    }

    private sealed class CardNode {
        public Card card;

        public CardNode up;
        public CardNode right;
        public CardNode down;
        public CardNode left;

        public CardNode(Card card) {
            this.card = card;
        }
    }
}
