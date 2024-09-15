using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractState : SimPlayerState {
        private GameObject interactable;

        public InteractState(GameObject interactable) => this.interactable = interactable;

        protected override void Enter() { }

        protected override void Exit() { }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Z)) {
                    nextState = new RegularState();
                }

                yield return null;
            }
        }
    }
}
