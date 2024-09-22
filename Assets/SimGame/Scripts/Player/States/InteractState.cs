using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractState : SimPlayerState {
        private Interactable interactable;
        private InteractionParser interactionParser;

        public InteractState(Interactable interactable) => this.interactable = interactable;

        protected override void Enter() {
            interactionParser = new InteractionParser(interactable);
            actor.StartCoroutine(interactionParser.BeginInteraction());

            player.SimGameManager.EventBus.OnStartInteract.Raise(new(interactionParser));
        }

        protected override void Exit() { }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    nextState = new RegularState(interactable);
                }

                if (Input.GetKeyDown(KeyCode.Z)) {
                    bool interactionComplete = interactionParser.HandleAdvance();

                    if (interactionComplete) {
                        player.SimGameManager.EventBus.OnEndInteract.Raise(EventArgs.Empty);
                        nextState = new RegularState(interactable);
                    }
                }

                yield return null;
            }
        }
    }
}
