using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class BoundEdgeSetter : MonoBehaviour {
        [SerializeField] private GameObject edgeContainer;

        private PolygonCollider2D bounds;
        private EdgeCollider2D[] edges;

        void Awake() {
            bounds = GetComponent<PolygonCollider2D>();
            Vector2[] path = bounds.GetPath(0);

            edges = edgeContainer.GetComponents<EdgeCollider2D>();
            if (edges.Length != path.Length) {
                foreach (EdgeCollider2D edge in edges) {
                    Destroy(edge);
                }

                edges = new EdgeCollider2D[path.Length];
            }

            for (int i = 0; i < path.Length; i++) {
                Vector2 currPoint = path[i];
                Vector2 nextPoint = path[(i + 1) % path.Length];

                if (edges[i] == null) {
                    edges[i] = edgeContainer.AddComponent<EdgeCollider2D>();
                }

                edges[i].points = new[] { currPoint, nextPoint };
            }
        }
    }
}
