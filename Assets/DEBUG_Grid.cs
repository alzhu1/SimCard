using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DEBUG_Grid : MonoBehaviour {
    [SerializeField] private int spacing = 16;

    void OnDrawGizmos() {
        Gizmos.color = Color.green;

        int limit = 1000 * spacing;
        int offset = spacing / 2;

        for (int s = -limit - offset; s <= limit - offset; s+= spacing) {
            // Vertical
            Gizmos.DrawLine(new(s, -limit), new(s, limit));

            // Horizontal
            Gizmos.DrawLine(new(-limit, s), new(limit, s));
        }
    }
}
