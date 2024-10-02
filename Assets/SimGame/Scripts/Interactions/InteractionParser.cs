using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractionParser {
        private Interactable interactable;

        public bool Completed => CurrInteraction == null;

        public Interaction CurrInteraction {
            get {
                List<Interaction> interactions = interactable.InteractableSO.interactions;
                if (interactionIndex >= interactions.Count) {
                    return null;
                }

                return interactions[interactionIndex];
            }
        }
        public float MaxInteractionTime {
            get {
                if (CurrInteraction == null) {
                    return 0;
                }
                return CurrInteraction.text.Length * CurrInteraction.TypeTime;
            }
        }

        public float CurrInteractionTime { get; private set; }

        private int interactionIndex = 0;

        public InteractionParser(Interactable interactable) => this.interactable = interactable;

        public IEnumerator HandleInteraction() {
            while (CurrInteraction != null) {
                if (CurrInteractionTime >= MaxInteractionTime) {
                    if (interactionIndex == interactable.InteractableSO.interactions.Count - 1) {
                        interactionIndex++;
                    }

                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(CurrInteraction.TypeTime);
                CurrInteractionTime += CurrInteraction.TypeTime;
            }
        }

        public void HandleAdvance() {
            if (CurrInteractionTime < MaxInteractionTime) {
                CurrInteractionTime = MaxInteractionTime;

                return;
            }

            // TODO: When HandleAdvance is called on last interaction, it should close dialogue
            // However, because the check is done in InteractState, we have to press again to exit interact state
            // Should fix this, i.e. on the last interaction, we want the HandleAdvance to assist in ending the interaction

            interactionIndex++;
            CurrInteractionTime = 0;
        }
    }
}
