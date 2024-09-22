using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractionParser {
        private Interactable interactable;

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
                return CurrInteraction.text.Length * CurrInteraction.typeTime;
            }
        }

        public float CurrInteractionTime { get; private set; }

        private int interactionIndex = 0;

        public InteractionParser(Interactable interactable) => this.interactable = interactable;

        public IEnumerator BeginInteraction() {
            while (CurrInteraction != null) {
                if (CurrInteractionTime >= MaxInteractionTime) {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(CurrInteraction.typeTime);
                CurrInteractionTime += CurrInteraction.typeTime;
            }

            // TODO: Need to send some signal to
        }

        public bool HandleAdvance() {
            // TODO: Thinking about make this signal based
            // So HandleAdvance will just set a signal, and the signal is handled in coroutine
            if (CurrInteractionTime < MaxInteractionTime) {
                CurrInteractionTime = MaxInteractionTime;
                return false;
            }

            interactionIndex++;
            CurrInteractionTime = 0;

            // Interaction ended if CurrInteraction is null
            return CurrInteraction == null;
        }
    }
}
