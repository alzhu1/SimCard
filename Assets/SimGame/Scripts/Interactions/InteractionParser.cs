using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractionParser : InteractionParser.InteractionParserUIListener {
        // Presenter interface for UI to work with
        // This allows us to limit the methods it will work with
        public interface InteractionParserUIListener {
            public Interaction CurrInteraction { get; }
            public int MaxVisibleCharacters { get; }
            public int OptionIndex { get; }
        }

        private Interactable interactable;
        private HashSet<string> traversedPaths;
        private string pathName;
        private int interactionIndex;

        // Events that the parser can call
        private GameEventAction<Args<List<string>>> DisplayInteractionOptions;
        private GameEventAction<Args<Interactable, string>> InteractionEvent;

        // Properties common to UI
        public Interaction CurrInteraction =>
            interactable.GetCurrentInteraction(pathName, interactionIndex);
        public int MaxVisibleCharacters { get; private set; }
        public int OptionIndex { get; private set; }

        public bool Completed { get; private set; }

        private int InteractionTextLength =>
            CurrInteraction == null ? 0 : CurrInteraction.text.Length;

        public InteractionParser(
            Interactable interactable,
            GameEventAction<Args<List<string>>> DisplayInteractionOptions,
            GameEventAction<Args<Interactable, string>> InteractionEvent
        ) {
            this.interactable = interactable;
            this.DisplayInteractionOptions = DisplayInteractionOptions;
            this.InteractionEvent = InteractionEvent;

            traversedPaths = new HashSet<string>();
            pathName = this.interactable.InitInteraction();
            interactionIndex = 0;
            traversedPaths.Add(pathName);
        }

        public YieldInstruction Tick() {
            if (CurrInteraction == null) {
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
            if (MaxVisibleCharacters < InteractionTextLength) {
                UpdateMaxVisibleCharacters(InteractionTextLength);
                return;
            }

            // HandleAdvance means we picked an option
            if (CurrInteraction.options.Count > 0) {
                pathName = interactable.ProcessInteractionOption(
                    pathName,
                    interactionIndex,
                    OptionIndex
                );
                interactionIndex = 0;
                OptionIndex = 0;
                MaxVisibleCharacters = 0;

                traversedPaths.Add(pathName);

                // Hide the options UI once an option is picked
                DisplayInteractionOptions.Raise(null);
                return;
            }

            // If tags indicate that the interaction is not renderable, keep incrementing
            do {
                interactionIndex++;
            } while (!interactable.ProcessInteractionTags(pathName, interactionIndex));

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
            InteractionPath interactionPath = interactable.GetCurrentInteractionPath(pathName);
            if (interactionPath.endingEventTriggers.Count > 0) {
                foreach (string eventTrigger in interactionPath.endingEventTriggers) {
                    InteractionEvent.Raise(new(interactable, eventTrigger));
                }
            }

            interactable.EndInteraction(traversedPaths);
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
                if (CurrInteraction.eventTriggers.Count > 0) {
                    foreach (string eventTrigger in CurrInteraction.eventTriggers) {
                        InteractionEvent.Raise(new(interactable, eventTrigger));
                    }
                }
            }
        }
    }
}
