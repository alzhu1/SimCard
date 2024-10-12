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

        private Dictionary<string, InteractionPath> interactionPaths = new();

        // Metadata
        private Dictionary<string, int> pathTraversedCount = new();

        public string CurrInteractionPath { get; private set; } = "Default";

        void Awake() {
            interactionPaths = InteractableSO.interactionPaths.ToDictionary(x => x.name, x => x);
            Assert.IsFalse(interactionPaths.ContainsKey("Default"));
            interactionPaths.Add(
                "Default",
                InteractionPath.CreateDefaultPath(InteractableSO.defaultInteractions)
            );

            // Ensure that all paths with positive starting priority is unique
            int uniqueStartingPriorityCount = interactionPaths
                .Values.Where(x => x.startingPriority > 0)
                .ToHashSet()
                .Count;
            Assert.AreEqual(interactionPaths.Count, uniqueStartingPriorityCount);
        }

        public void InitInteraction() {
            CurrInteractionPath = "Default";
            int priority = 0;

            foreach (InteractionPath path in interactionPaths.Values) {
                if (path.startingPriority > priority && ProcessTagsOn(path.pathTags)) {
                    CurrInteractionPath = path.name;
                    priority = path.startingPriority;
                }
            }
        }

        public void EndInteraction(HashSet<string> traversedPaths) {
            // Keep track of path counts
            foreach (string traversedPath in traversedPaths) {
                int pathCount = pathTraversedCount.GetValueOrDefault(traversedPath);
                pathTraversedCount[traversedPath] = pathCount + 1;
            }
        }

        public Interaction GetCurrentInteraction(int interactionIndex) {
            return interactionPaths
                .GetValueOrDefault(CurrInteractionPath)
                ?.interactions.ElementAtOrDefault(interactionIndex);
        }

        public bool ProcessInteractionTags(int interactionIndex) {
            Interaction currentInteraction = GetCurrentInteraction(interactionIndex);
            if (currentInteraction == null) {
                return true;
            }

            return ProcessTagsOn(currentInteraction.tags);
        }

        public void ProcessInteractionOption(int interactionIndex, int optionIndex) {
            InteractionOption currOption = GetCurrentInteraction(interactionIndex)
                ?.options[optionIndex];
            Assert.IsNotNull(currOption);

            // If the next path contains some conditional tags, go there only if conditions are met
            // For now, just assume a singular fallback path
            bool toNextPath = ProcessTagsOn(
                interactionPaths.GetValueOrDefault(currOption.nextInteractionPath)?.pathTags
            );
            CurrInteractionPath = toNextPath
                ? currOption.nextInteractionPath
                : currOption.fallbackInteractionPath;
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

                    case ScriptTag.PathTraversedCount: {
                        int requiredTraversedCount = interactionTag.intArg;
                        string pathName = interactionTag.strArg;
                        if (pathTraversedCount.GetValueOrDefault(pathName) < requiredTraversedCount) {
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
