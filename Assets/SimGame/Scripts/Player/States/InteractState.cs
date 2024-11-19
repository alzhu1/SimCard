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
            interactionParser = new InteractionParser(
                interactable,
                player.SimGameManager.EventBus.OnDisplayInteractOptions,
                player.SimGameManager.EventBus.OnInteractionEvent
            );
        }

        protected override void Exit() { }

        protected override IEnumerator Handle() {
            yield return player.SimGameManager.StartInteractionCoroutine(interactionParser);
            actor.StartCoroutine(HandleInputs());

            while (nextState == null) {
                yield return interactionParser.Tick();

                if (interactionParser.Completed) {
                    nextState = new RegularState(interactable);
                }
            }

            // When complete, wait for UI before cleaning up
            yield return player.SimGameManager.EndInteractionCoroutine(interactionParser);
        }

        IEnumerator HandleInputs() {
            while (nextState == null) {
                if (Input.GetKeyDown(KeyCode.Z)) {
                    interactionParser.HandleAdvance();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    interactionParser.UpdateOptionIndex(-1);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    interactionParser.UpdateOptionIndex(1);
                }

                yield return null;
            }
        }
    }
}
