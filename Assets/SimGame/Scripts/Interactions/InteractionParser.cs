using System;
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
            public int OptionIndex { get; }

            public void NotifyFromUI();
        }

        private Interactable interactable;

        // UI wait utils
        private bool waitingForUI = true;
        public CustomYieldInstruction WaitForUI => new WaitWhile(() => waitingForUI);

        // Properties common to UI
        public Interaction CurrInteraction => interactable.GetCurrentInteraction(interactionIndex);
        public int MaxVisibleCharacters { get; private set; }
        public int OptionIndex { get; private set; }

        public bool Completed { get; private set; }

        public int InteractionTextLength =>
            CurrInteraction == null ? 0 : CurrInteraction.text.Length;

        private int interactionIndex = 0;

        // Events that the parser can call
        public Action<Args<List<string>>> DisplayInteractionOptions;

        public InteractionParser(
            Interactable interactable,
            Action<Args<List<string>>> DisplayInteractionOptions
        ) {
            this.interactable = interactable;
            this.DisplayInteractionOptions = DisplayInteractionOptions;

            this.interactable.InitInteraction();
        }

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
            if (MaxVisibleCharacters >= InteractionTextLength && CurrInteraction.options.Count > 0) {
                DisplayInteractionOptions?.Invoke(
                    new(CurrInteraction.options.Select(x => x.option).ToList())
                );
            }

            return new WaitForSeconds(interactable.TypeTime);
        }

        public void HandleAdvance() {
            if (waitingForUI) {
                return;
            }

            if (MaxVisibleCharacters < InteractionTextLength) {
                MaxVisibleCharacters = InteractionTextLength;

                if (CurrInteraction.options.Count > 0) {
                    DisplayInteractionOptions?.Invoke(
                        new(CurrInteraction.options.Select(x => x.option).ToList())
                    );
                }
                return;
            }

            // HandleAdvance means we picked an option
            if (CurrInteraction.options.Count > 0) {
                interactable.ProcessInteractionOption(interactionIndex, OptionIndex);
                interactionIndex = 0;
                OptionIndex = 0;
                MaxVisibleCharacters = 0;

                // Hide the options UI once an option is picked
                DisplayInteractionOptions?.Invoke(null);
                return;
            }

            // If tags indicate that the interaction is not renderable, keep incrementing
            do {
                interactionIndex++;
            } while (!interactable.ProcessInteractionTags(interactionIndex));

            MaxVisibleCharacters = 0;
        }

        public void UpdateOptionIndex(int diff) {
            if (MaxVisibleCharacters < InteractionTextLength) {
                return;
            }

            int optionCount = CurrInteraction.options.Count;
            if (optionCount > 0) {
                OptionIndex = (optionCount + OptionIndex + diff) % optionCount;
            }
        }

        public void NotifyFromUI() {
            waitingForUI = false;
        }
    }
}
