using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractableCharacterAnimator : MonoBehaviour {
        [SerializeField]
        private string[] idleAnimationStates = new string[] {
            "Character_Walk_Down",
            "Character_Walk_Up",
            "Character_Walk_Side"
        };

        [SerializeField]
        private int startingIndex = 0;

        [SerializeField]
        private bool shouldReturnToStart = true;
        public bool ShouldReturnToStart => shouldReturnToStart;

        private Animator animator;
        private SpriteRenderer sr;

        private string currState;
        private bool startingFlipX;

        void Awake() {
            animator = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();

            currState = idleAnimationStates[startingIndex];
            startingFlipX = sr.flipX;

            animator.Play(currState);
        }

        void OnEnable() {
            // Keep on with current state
            animator.Play(currState);
        }

        public void ReturnToStart() {
            sr.flipX = startingFlipX;
            currState = idleAnimationStates[startingIndex];

            animator.Play(currState);
        }

        public void HandleDirectionChange(Vector2 faceDirection) {
            string nextState = faceDirection switch {
                Vector2 d when d.y < 0 => idleAnimationStates[0],
                Vector2 d when d.y > 0 => idleAnimationStates[1],
                Vector2 d when d.x > 0 || d.x < 0 => idleAnimationStates[2],
                _ => currState
            };

            // Flip only if we show side to the left
            sr.flipX = faceDirection.x < 0;

            if (!currState.Equals(nextState)) {
                animator.Play(nextState);
            }
            currState = nextState;
        }
    }
}
