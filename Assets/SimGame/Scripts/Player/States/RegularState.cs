using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class RegularState : SimPlayerState {
        private Vector3 move;

        private Interactable interactable;

        public RegularState() { }

        public RegularState(Interactable interactable) {
            this.interactable = interactable;
            interactable.HandlePopupVisibility(true);
        }

        public override Vector2 RBVelocity => player.MoveSpeed * Time.fixedDeltaTime * move;

        protected override void Enter() {
            base.Enter();
            player.SimGameManager.EventBus.OnCanInteract.Event += EnableInteraction;
        }

        protected override void Exit() {
            base.Exit();
            player.SimGameManager.EventBus.OnCanInteract.Event -= EnableInteraction;
        }

        protected override IEnumerator Handle() {
            // Do a quick front collider check (only if we have no interactable)
            if (interactable == null) {
                player.FrontCheckCollider.enabled = false;
                yield return null;
                player.FrontCheckCollider.enabled = true;
            }

            while (nextState == null) {
                if (interactable != null && Input.GetKeyDown(KeyCode.Space)) {
                    nextState = new InteractState(interactable);
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Escape)) {
                    interactable?.HandlePopupVisibility(false);
                    nextState = new MenuState();
                    break;
                }

                Vector2 prevMove = move;

                move = new Vector2(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical")
                ).normalized;

                bool isPrevMoveStopped = prevMove.Equals(Vector2.zero);
                bool isCurrMoveStopped = move.Equals(Vector2.zero);

                // i.e. (came to a stop) OR (started moving from idle position)
                if ((!isPrevMoveStopped && isCurrMoveStopped) || (isPrevMoveStopped && !isCurrMoveStopped)) {
                    HandleAnimator(player.PlayerDirection);
                }

                SetDirection(move);
                yield return null;
            }
        }

        void SetDirection(Vector3 nextMove) {
            if (nextMove.Equals(Vector3.zero)) {
                return;
            }

            Vector3 direction = player.PlayerDirection;

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
                    _ => direction,
                };
            }

            // Handles directional change, not stop->start change
            HandleAnimator(direction);

            player.FrontCheck.localPosition = direction;

            Vector2 tempSize = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
            Vector2 oppositeSize = new Vector2(tempSize.y, tempSize.x) * 0.2f;
            player.FrontCheckCollider.size = tempSize + oppositeSize;
        }

        void HandleAnimator(Vector3 direction) {
            string prefix = RBVelocity.magnitude > 0 ? "Character_Walk" : "Character_Idle";

            if (direction.x < 0) {
                player.SR.flipX = true;
                player.Animator.Play($"{prefix}_Side");
            } else {
                player.SR.flipX = false;

                if (direction.x > 0) {
                    player.Animator.Play($"{prefix}_Side");
                } else if (direction.y < 0) {
                    player.Animator.Play($"{prefix}_Down");
                } else if (direction.y > 0) {
                    player.Animator.Play($"{prefix}_Up");
                }
            }
        }

        void EnableInteraction(EventArgs<Interactable> args) {
            interactable = args.argument;
        }
    }
}
