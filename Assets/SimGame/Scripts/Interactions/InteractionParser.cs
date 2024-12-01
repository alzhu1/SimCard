using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractionParser : InteractUIListener, OptionsUIListener {
        private Player player;
        private Interactable interactable;
        private HashSet<string> traversedPaths;
        private List<int> validOptionIndices;
        private string pathName;
        private int interactionIndex;
        private int validOptionIndex;

        // Events that the parser can call
        private GameEventAction<EventArgs<OptionsUIListener, List<(string, bool)>>> DisplayInteractionOptions;
        private GameEventAction<EventArgs<Interactable, string>> InteractionEvent;

        // Properties common to UI
        public Interaction CurrInteraction =>
            interactable.GetCurrentInteraction(pathName, interactionIndex);
        public int MaxVisibleCharacters { get; private set; }
        public int OptionIndex { get; private set; }

        public bool Completed { get; private set; }

        private int InteractionTextLength =>
            CurrInteraction == null ? 0 : CurrInteraction.text.Length;

        public InteractionParser(
            Player player,
            Interactable interactable,
            GameEventAction<EventArgs<OptionsUIListener, List<(string, bool)>>> DisplayInteractionOptions,
            GameEventAction<EventArgs<Interactable, string>> InteractionEvent
        ) {
            this.player = player;
            this.interactable = interactable;
            this.DisplayInteractionOptions = DisplayInteractionOptions;
            this.InteractionEvent = InteractionEvent;

            traversedPaths = new HashSet<string>();
            validOptionIndices = new List<int>();
            pathName = this.interactable.InitInteraction();
            interactionIndex = 0;
            validOptionIndex = 0;
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
            List<InteractionOption> options = CurrInteraction.options;
            if (options.Count > 0) {
                pathName = interactable.ProcessInteractionOption(
                    pathName,
                    interactionIndex,
                    OptionIndex
                );

                interactionIndex = 0;
                MaxVisibleCharacters = 0;

                // Consume option's energy
                player.ConsumeEnergy(options[OptionIndex].energyCost);

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

            validOptionIndex = (validOptionIndices.Count + validOptionIndex + diff) % validOptionIndices.Count;
            OptionIndex = validOptionIndices[validOptionIndex];
        }

        public void EndInteraction() {
            InteractionPath interactionPath = interactable.GetCurrentInteractionPath(pathName);
            if (interactionPath?.endingEventTriggers?.Count > 0) {
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
                List<InteractionOption> options = CurrInteraction.options;
                if (options.Count > 0) {
                    List<(string, bool)> optionsAllowed = options.Select(x => (x.option, x.energyCost <= player.Energy)).ToList();
                    DisplayInteractionOptions.Raise(new(this, optionsAllowed));

                    validOptionIndices.Clear();
                    validOptionIndex = 0;
                    for (int i = 0; i < optionsAllowed.Count; i++) {
                        if (optionsAllowed[i].Item2) {
                            validOptionIndices.Add(i);
                        }
                    }

                    OptionIndex = validOptionIndices[0];
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
