using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimCard.Common;

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
        private HashSet<string> traversedPaths;

        // Events that the parser can call
        public GameEventAction<Args<List<string>>> DisplayInteractionOptions;
        public GameEventAction<Args<string>> InteractionEvent;

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

        public InteractionParser(
            Interactable interactable,
            GameEventAction<Args<List<string>>> DisplayInteractionOptions,
            GameEventAction<Args<string>> InteractionEvent
        ) {
            this.interactable = interactable;
            this.DisplayInteractionOptions = DisplayInteractionOptions;
            this.InteractionEvent = InteractionEvent;
            traversedPaths = new HashSet<string>();

            this.interactable.InitInteraction();
            traversedPaths.Add(this.interactable.CurrInteractionPathName);
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

            UpdateMaxVisibleCharacters(MaxVisibleCharacters + 1);
            return new WaitForSeconds(interactable.TypeTime);
        }

        public void HandleAdvance() {
            if (waitingForUI) {
                return;
            }

            if (MaxVisibleCharacters < InteractionTextLength) {
                UpdateMaxVisibleCharacters(InteractionTextLength);
                return;
            }

            // HandleAdvance means we picked an option
            if (CurrInteraction.options.Count > 0) {
                interactable.ProcessInteractionOption(interactionIndex, OptionIndex);
                interactionIndex = 0;
                OptionIndex = 0;
                MaxVisibleCharacters = 0;

                traversedPaths.Add(interactable.CurrInteractionPathName);

                // Hide the options UI once an option is picked
                DisplayInteractionOptions.Raise(null);
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

        public void EndInteraction() {
            if (interactable.CurrInteractionPath.endingEventTriggers.Count > 0) {
                foreach (string eventTrigger in interactable.CurrInteractionPath.endingEventTriggers) {
                    InteractionEvent.Raise(new(eventTrigger));
                }
            }

            interactable.EndInteraction(traversedPaths);
        }

        public void NotifyFromUI() {
            waitingForUI = false;
        }

        // Update character value + associated events
        void UpdateMaxVisibleCharacters(int value) {
            MaxVisibleCharacters = value;

            if (MaxVisibleCharacters >= InteractionTextLength) {
                // Display options
                if (CurrInteraction.options.Count > 0) {
                    DisplayInteractionOptions.Raise(
                        new(CurrInteraction.options.Select(x => x.option).ToList())
                    );
                }

                // Event triggers for interaction
                // TODO: Need a way to set the event trigger granularly
                // e.g. want it at end of interaction
                if (CurrInteraction.eventTriggers.Count > 0) {
                    foreach (string eventTrigger in CurrInteraction.eventTriggers) {
                        InteractionEvent.Raise(new(eventTrigger));
                    }
                }
            }
        }
    }
}
