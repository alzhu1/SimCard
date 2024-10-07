using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public Interaction CurrInteraction =>
            interactable.InteractionPaths.GetValueOrDefault(interactable.InteractionPath)?.ElementAtOrDefault(interactionIndex);

        public int MaxVisibleCharacters { get; private set; }

        public int InteractionTextLength =>
            CurrInteraction == null ? 0 : CurrInteraction.text.Length;

        private int interactionIndex = 0;
        private bool waitingForUI = true;

        public CustomYieldInstruction WaitForUI => new WaitWhile(() => waitingForUI);

        public InteractionParser(Interactable interactable) => this.interactable = interactable;

        public YieldInstruction Tick() {
            if (CurrInteraction == null) {
                waitingForUI = true;
                Completed = true;
                return null;
            }

            if (MaxVisibleCharacters >= InteractionTextLength) {
                return null;
            }

            MaxVisibleCharacters++;
            return new WaitForSeconds(interactable.TypeTime);
        }

        public void HandleAdvance() {
            if (waitingForUI) {
                return;
            }

            if (MaxVisibleCharacters < InteractionTextLength) {
                MaxVisibleCharacters = InteractionTextLength;
                return;
            }

            // TODO: Check if there are options
            if (CurrInteraction.options.Count > 0) {
                // HandleAdvance means we picked an option
                interactable.InteractionPath = CurrInteraction.options[0].nextInteractionPath;
                interactionIndex = 0;
                MaxVisibleCharacters = 0;
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
