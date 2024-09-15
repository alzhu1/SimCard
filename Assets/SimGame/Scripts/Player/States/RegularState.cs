using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class RegularState : SimPlayerState {
        private Vector3 move;
        private Vector3Int direction;

        private GameObject interactable;

        public RegularState() { }
        public RegularState(GameObject interactable) => this.interactable = interactable;

        public override Vector2 RBVelocity => player.MoveSpeed * Time.fixedDeltaTime * move;

        protected override void Enter() {
            player.SimGameManager.EventBus.OnCanInteract.Event += EnableInteraction;
        }

        protected override void Exit() {
            player.SimGameManager.EventBus.OnCanInteract.Event -= EnableInteraction;
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (interactable != null && Input.GetKeyDown(KeyCode.Z)) {
                    nextState = new InteractState(interactable);
                    break;
                }

                Vector3 nextMove = new Vector2(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical")
                ).normalized;

                SetDirection(nextMove);

                move = nextMove;
                yield return null;
            }
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
            if (priorityValue > 0f) {
                return;
            }

            if (priorityValue < 0f) {
                // Go in oppo direction
                direction *= -1;
            } else {
                direction = nextMove switch {
                    Vector3 v when v.x > 0 => Vector3Int.right,
                    Vector3 v when v.x < 0 => Vector3Int.left,
                    Vector3 v when v.y > 0 => Vector3Int.up,
                    Vector3 v when v.y < 0 => Vector3Int.down,
                    _ => direction
                };
            }

            player.FrontCheck.localPosition = direction;
        }

        void EnableInteraction(InteractArgs args) {
            interactable = args.interactable;
        }
    }
}
