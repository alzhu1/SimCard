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
        private string pathName;
        private int interactionIndex;

        // Events that the parser can call
        private GameEventAction<EventArgs<OptionsUIListener, List<string>>> DisplayInteractionOptions;
        private GameEventAction<EventArgs<OptionsUIListener, List<CardMetadata>, Player>> DisplayShopOptions;
        private GameEventAction<EventArgs<string, Interactable, int>> InteractionEvent;

        private InteractionNode CurrInteractionNode => interactable.GetCurrentInteraction(pathName, interactionIndex);
        private int InteractionTextLength => CurrInteractionText == null ? 0 : CurrInteractionText.Length;

        // Properties common to UI
        public string CurrInteractionText => interactable.GetCurrentInteraction(pathName, interactionIndex)?.Text;
        public int MaxVisibleCharacters { get; private set; }
        public int OptionIndex { get; private set; }

        public bool Completed { get; private set; }

        public InteractionParser(Player player, Interactable interactable) {
            this.player = player;
            this.interactable = interactable;

            // Initialize events
            DisplayInteractionOptions = player.SimGameManager.EventBus.OnDisplayInteractOptions;
            DisplayShopOptions = player.SimGameManager.EventBus.OnDisplayShopOptions;
            InteractionEvent = player.SimGameManager.EventBus.OnInteractionEvent;

            // Initialize other data
            traversedPaths = new HashSet<string>();
            pathName = this.interactable.InitInteraction(player);
            interactionIndex = 0;
            traversedPaths.Add(pathName);
        }

        public YieldInstruction Tick() {
            if (CurrInteractionText == null) {
                Completed = true;
                return null;
            }

            if (MaxVisibleCharacters >= InteractionTextLength) {
                return null;
            }

            UpdateMaxVisibleCharacters(MaxVisibleCharacters + 1);
            return new WaitForSeconds(interactable.TypeTime);
        }

        public void HandleAdvance(bool active, System.Action PlaySFX) {
            if (active && MaxVisibleCharacters < InteractionTextLength) {
                UpdateMaxVisibleCharacters(InteractionTextLength);
                return;
            }

            if (!active) {
                // Only support inactive ("close" button) input for shop buy
                if (pathName.StartsWith("$ShopBuy")) {
                    PlaySFX();

                    pathName = "Default";
                    interactionIndex = 0;
                    MaxVisibleCharacters = 0;
                    DisplayInteractionOptions.Raise(null);
                    DisplayShopOptions.Raise(null);
                }
                return;
            }

            PlaySFX();

            // HandleAdvance means we picked an option
            List<InteractionNode.InteractionOption> options = CurrInteractionNode.Options;

            if (options.Count > 0) {
                pathName = interactable.ProcessInteractionOption(
                    pathName,
                    interactionIndex,
                    OptionIndex
                );

                interactionIndex = 0;
                MaxVisibleCharacters = 0;

                traversedPaths.Add(pathName);

                // Hide the options UI once an option is picked
                DisplayInteractionOptions.Raise(null);

                // We could check for special paths names (starting with $) and add handling there
                Debug.Log($"Chosen path name: {pathName}");
                if (pathName.StartsWith("$ShopBuy")) {
                    Debug.LogWarning("We are on a shop buy path");

                    // Raise the shop buy event to immediately show the shop view
                    DisplayShopOptions.Raise(new(this, interactable.Deck, player));

                    // Immediately show all text, too
                    UpdateMaxVisibleCharacters(InteractionTextLength);
                }

                return;
            }

            // If tags indicate that the interaction is not renderable, keep incrementing
            do {
                interactionIndex++;
            } while (!interactable.ProcessInteractionConditions(pathName, interactionIndex));

            MaxVisibleCharacters = 0;
        }

        public void UpdateOptionIndex(int diff, System.Action PlaySFX) {
            if (MaxVisibleCharacters < InteractionTextLength) {
                return;
            }

            PlaySFX();
            int optionCount = CurrInteractionNode.Options.Count;
            OptionIndex = (optionCount + OptionIndex + diff) % optionCount;
        }

        public void EndInteraction() {
            // Check the last value of the last path
            InteractionNode lastNode = interactable.GetCurrentInteraction(pathName, interactionIndex - 1);
            if (lastNode?.EndingEventTriggers.Count > 0) {
                foreach (string eventTrigger in lastNode.EndingEventTriggers) {
                    InteractionEvent.Raise(new(eventTrigger, interactable, 0));
                }
            }

            interactable.EndInteraction(traversedPaths.Where(path => path != null));
        }

        // Update character value + associated events
        void UpdateMaxVisibleCharacters(int value) {
            MaxVisibleCharacters = value;

            if (MaxVisibleCharacters >= InteractionTextLength) {
                // Event triggers for interaction
                if (CurrInteractionNode?.EventTriggers?.Count > 0) {
                    foreach (string eventTrigger in CurrInteractionNode.EventTriggers) {
                        InteractionEvent.Raise(new(eventTrigger, interactable, OptionIndex));
                    }
                }

                // Display options
                List<InteractionNode.InteractionOption> options = CurrInteractionNode.Options;
                if (options?.Count > 0) {
                    // For shop interactions, don't reset the index
                    if (!pathName.StartsWith("$ShopBuy")) {
                        DisplayInteractionOptions.Raise(new(this, options.Select(x => x.OptionText).ToList()));
                        OptionIndex = 0;
                    }
                }
            }
        }
    }
}
