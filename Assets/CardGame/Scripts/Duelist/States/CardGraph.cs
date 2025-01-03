using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimCard.CardGame {
    public interface CardGraphSelectable {
        // Unity components
        public Transform transform { get; }

        // Properties
        public string PreviewName { get; }
    }

    public class CardGraph<T> where T : class, CardGraphSelectable {

        private Node currNode;

        public T CurrItem => currNode.item;

        public CardGraph(List<List<T>> selectablesByRow, T startingItem) {
            // First, iterate through all cards by row, replace with Node
            // Set left/right

            List<List<Node>> nodesByRow = selectablesByRow
                .Where(row => row.Count > 0)
                .Select(row => row.Select(card => new Node(card)).ToList())
                .ToList();
            int rowCount = nodesByRow.Count;

            for (int i = 0; i < rowCount; i++) {
                List<Node> row = nodesByRow[i];

                // Get prev/next row to set for up/down (should loop)
                List<Node> prevRow = nodesByRow[(rowCount + i - 1) % rowCount];
                List<Node> nextRow = nodesByRow[(i + 1) % rowCount];

                for (int j = 0; j < row.Count; j++) {
                    Node node = row[j];

                    if (node.item == startingItem) {
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

        private sealed class Node {
            public T item;

            public Dictionary<Vector2Int, Node> dir =
                new()
                {
                    { Vector2Int.up, null },
                    { Vector2Int.right, null },
                    { Vector2Int.down, null },
                    { Vector2Int.left, null },
                };

            public Node Up {
                get { return dir[Vector2Int.up]; }
                set { dir[Vector2Int.up] = value; }
            }

            public Node Right {
                get { return dir[Vector2Int.right]; }
                set { dir[Vector2Int.right] = value; }
            }

            public Node Down {
                get { return dir[Vector2Int.down]; }
                set { dir[Vector2Int.down] = value; }
            }

            public Node Left {
                get { return dir[Vector2Int.left]; }
                set { dir[Vector2Int.left] = value; }
            }

            public Node(T item) {
                this.item = item;
            }
        }
    }
}
