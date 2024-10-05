using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractionParser : InteractionParser.InteractionParserUIListener {
        // Presenter interface for UI to work with
        // This allows us to limit the methods it will work with
        public interface InteractionParserUIListener {
            public Interaction CurrInteraction { get; }
            public int MaxVisibleCharacters { get; }

            public void NotifyFromUI();
        }

        private Interactable interactable;

        public bool Completed { get; private set; }

        public Interaction CurrInteraction {
            get {
                List<Interaction> interactions = interactable.InteractableSO.interactions;
                if (interactionIndex >= interactions.Count) {
                    return null;
                }

                return interactions[interactionIndex];
            }
        }

        public int InteractionTextLength =>
            CurrInteraction == null ? 0 : CurrInteraction.text.Length;

        public int MaxVisibleCharacters { get; private set; }

        private int interactionIndex = 0;
        private bool waitingForUI = false;

        public InteractionParser(Interactable interactable) => this.interactable = interactable;

        public IEnumerator HandleInteraction() {
            waitingForUI = true;
            yield return new WaitWhile(() => waitingForUI);

            while (CurrInteraction != null) {
                if (MaxVisibleCharacters >= InteractionTextLength) {
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(CurrInteraction.TypeTime);
                MaxVisibleCharacters++;
            }

            waitingForUI = true;
            yield return new WaitWhile(() => waitingForUI);

            Completed = true;
        }

        public void HandleAdvance() {
            if (waitingForUI) {
                return;
            }

            if (MaxVisibleCharacters < InteractionTextLength) {
                MaxVisibleCharacters = InteractionTextLength;
                return;
            }

            interactionIndex++;
            MaxVisibleCharacters = 0;
        }

        public void NotifyFromUI() {
            waitingForUI = false;
        }
    }
}
