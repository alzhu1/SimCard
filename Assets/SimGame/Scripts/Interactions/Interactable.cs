using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SimCard.SimGame {
    public class Interactable : MonoBehaviour {
        private static float GlobalTypeTime => 0.08f;

        [SerializeField]
        private float typeTime;
        public float TypeTime => typeTime == 0 ? GlobalTypeTime : typeTime;

        [field: SerializeField]
        public InteractableSO InteractableSO { get; private set; }

        public Dictionary<string, InteractionPath> InteractionPaths { get; private set; }

        private string interactionPath = "Default";

        void Awake() {
            InteractionPaths = InteractableSO.interactionPaths.ToDictionary(
                x => x.name,
                x => x
            );
            Assert.IsFalse(InteractionPaths.ContainsKey("Default"));
            InteractionPaths.Add("Default", InteractionPath.CreateDefaultPath(InteractableSO.defaultInteractions));
        }

        public void InitInteraction() {
            // TODO: Set the starting interaction path
            interactionPath = "Default";
        }

        public Interaction GetCurrentInteraction(int interactionIndex) {
            return InteractionPaths.GetValueOrDefault(interactionPath)?.interactions.ElementAtOrDefault(interactionIndex);
        }

        public bool ProcessInteractionTags(int interactionIndex) {
            Interaction currentInteraction = GetCurrentInteraction(interactionIndex);
            if (currentInteraction == null) {
                return true;
            }

            return ProcessTagsOn(currentInteraction.tags);
        }

        public void ProcessInteractionOption(int interactionIndex, int optionIndex) {
            InteractionOption currOption = GetCurrentInteraction(interactionIndex)?.options[optionIndex];
            Assert.IsNotNull(currOption);

            // If the next path contains some conditional tags, go there only if conditions are met
            // For now, just assume a singular fallback path
            if (ProcessTagsOn(InteractionPaths.GetValueOrDefault(currOption.nextInteractionPath)?.pathTags)) {
                interactionPath = currOption.nextInteractionPath;
            } else {
                interactionPath = currOption.fallbackInteractionPath;
            }
        }

        bool ProcessTagsOn(List<InteractionTag> interactionTags) {
            if (interactionTags == null) {
                return true;
            }

            foreach (InteractionTag interactionTag in interactionTags) {
                switch (interactionTag.tag) {
                    case ScriptTag.Bool: {
                        if (!interactionTag.boolArg) {
                            return false;
                        }
                        break;
                    }
                }
            }

            return true;
        }
    }
}
