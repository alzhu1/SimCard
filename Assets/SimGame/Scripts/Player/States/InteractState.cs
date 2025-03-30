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
            base.Enter();
            interactionParser = new InteractionParser(player, interactable);
        }

        protected override void Exit() {
            base.Exit();
        }

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
                if (Input.GetKeyDown(KeyCode.Space)) {
                    interactionParser.HandleAdvance(true, player.SimGameManager.PlayAdvanceSound);
                }

                if (Input.GetKeyDown(KeyCode.Escape)) {
                    interactionParser.HandleAdvance(false, player.SimGameManager.PlayAdvanceSound);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    interactionParser.UpdateOptionIndex(-1, player.SimGameManager.PlayOptionMoveSound);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    interactionParser.UpdateOptionIndex(1, player.SimGameManager.PlayOptionMoveSound);
                }

                yield return null;
            }
        }
    }
}
