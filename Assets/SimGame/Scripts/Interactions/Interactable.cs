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

        void Awake() {
            interactionPaths = InteractableSO.interactionPaths.ToDictionary(x => x.name, x => x);
            Assert.IsFalse(interactionPaths.ContainsKey("Default"));
            interactionPaths.Add(
                "Default",
                InteractionPath.CreateDefaultPath(InteractableSO.defaultInteractions)
            );

            // Ensure that all paths with positive starting priority is unique
            IEnumerable<int> startingPriorities = interactionPaths
                .Values.Where(x => x.startingPriority > 0)
                .Select(x => x.startingPriority);

            int allStartingPriorityCounts = startingPriorities.Count();
            int uniqueStartingPriorityCount = startingPriorities.ToHashSet().Count;

            Assert.AreEqual(uniqueStartingPriorityCount, allStartingPriorityCounts);
        }

        public string InitInteraction() {
            string startingPathName = "Default";
            int priority = 0;

            foreach (InteractionPath path in interactionPaths.Values) {
                if (path.startingPriority > priority && ProcessTagsOn(path.pathTags)) {
                    startingPathName = path.name;
                    priority = path.startingPriority;
                }
            }

            return startingPathName;
        }

        public void EndInteraction(HashSet<string> traversedPaths) {
            // Keep track of path counts
            foreach (string traversedPath in traversedPaths) {
                int pathCount = pathTraversedCount.GetValueOrDefault(traversedPath);
                pathTraversedCount[traversedPath] = pathCount + 1;
            }
        }

        public InteractionPath GetCurrentInteractionPath(string pathName) {
            return interactionPaths.GetValueOrDefault(pathName);
        }

        public Interaction GetCurrentInteraction(string pathName, int interactionIndex) {
            return GetCurrentInteractionPath(pathName)
                ?.interactions.ElementAtOrDefault(interactionIndex);
        }

        public bool ProcessInteractionTags(string pathName, int interactionIndex) {
            Interaction currentInteraction = GetCurrentInteraction(pathName, interactionIndex);
            if (currentInteraction == null) {
                return true;
            }

            return ProcessTagsOn(currentInteraction.tags);
        }

        public string ProcessInteractionOption(
            string pathName,
            int interactionIndex,
            int optionIndex
        ) {
            InteractionOption currOption = GetCurrentInteraction(pathName, interactionIndex)
                ?.options[optionIndex];
            Assert.IsNotNull(currOption);

            // If the next path contains some conditional tags, go there only if conditions are met
            // For now, just assume a singular fallback path
            bool toNextPath = ProcessTagsOn(
                interactionPaths.GetValueOrDefault(currOption.nextInteractionPath)?.pathTags
            );
            return toNextPath ? currOption.nextInteractionPath : currOption.fallbackInteractionPath;
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
                        string pathName = interactionTag.strArg;
                        int requiredTraversedCount = interactionTag.intArg;
                        int currentTraversedCount = pathTraversedCount.GetValueOrDefault(pathName);

                        if (
                            !interactionTag.ApplyCondition(
                                currentTraversedCount,
                                requiredTraversedCount
                            )
                        ) {
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
