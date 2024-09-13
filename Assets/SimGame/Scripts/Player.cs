using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class Player : MonoBehaviour {
        [SerializeField] private float moveSpeed = 1f;

        private Rigidbody2D rb;

        private Vector3 move;
        private Vector3Int direction;

        void Awake() {
            rb = GetComponent<Rigidbody2D>();
            direction = Vector3Int.down;
        }

        void Start() {

        }

        void Update() {
            Vector3 nextMove = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            var a = direction;
            SetDirection(nextMove);
            var b = direction;

            Debug.Log($"Prev dir = {a}, next dir = {b}");

            move = nextMove;
        }

        void FixedUpdate() {
            rb.velocity = moveSpeed * Time.fixedDeltaTime * move;
        }

        void SetDirection(Vector3 nextMove) {
            if (nextMove.Equals(Vector3.zero)) {
                return;
            }

            // Dot product should tell us what effect to use
            // Negative means moving in opposite direction of primary dir
            // Positive means moving in same direction, so no change
            // 0 means a possible change (not moving or moving perpendicular)
            float priorityValue = Vector3.Dot(direction, nextMove);
            if (priorityValue < 0f) {
                // Go in oppo direction
                direction = Vector3Int.zero - direction;
                return;
            } else if (priorityValue > 0f) {
                return;
            }

            direction = nextMove switch {
                Vector3 v when v.x > 0 => Vector3Int.right,
                Vector3 v when v.x < 0 => Vector3Int.left,
                Vector3 v when v.y > 0 => Vector3Int.up,
                Vector3 v when v.y < 0 => Vector3Int.down,
                _ => direction
            };
        }
    }
}
