using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class Interactable : MonoBehaviour {
        private static float GlobalTypeTime => 0.08f;

        [SerializeField]
        private float typeTime;
        public float TypeTime => typeTime == 0 ? GlobalTypeTime : typeTime;

        [SerializeField]
        private List<CardMetadata> deck;
        public List<CardMetadata> Deck => deck;

        [SerializeField] private TextAsset data;
        private Interaction interactionJson;

        // Metadata
        private Dictionary<string, int> pathTraversedCount = new();

        void Awake() {
            interactionJson = JObject.Parse(data.text).ToObject<Interaction>();
        }

        bool AssertCondition(ConditionKey condition, string parameter) {
            switch (condition) {
                case ConditionKey.Bool: {
                    return bool.Parse(parameter);
                }

                case ConditionKey.Energy: {
                    // TODO: Fix
                    return false;
                }

                case ConditionKey.PathTraversedCount: {
                    // TODO: Fix
                    return false;
                }

                default: {
                    return false;
                }
            }
        }

        public string InitInteraction() {
            foreach (InitInteraction.InitPathOptions pathOption in interactionJson.Init.PathOptions) {
                if (pathOption.Conditions.All(kv => AssertCondition(kv.Key, kv.Value))) {
                    return pathOption.NextPath;
                }
            }

            return "Default";
        }

        public void EndInteraction(HashSet<string> traversedPaths) {
            // Keep track of path counts
            foreach (string traversedPath in traversedPaths) {
                int pathCount = pathTraversedCount.GetValueOrDefault(traversedPath);
                pathTraversedCount[traversedPath] = pathCount + 1;
            }
        }

        public InteractionNode GetCurrentInteractionV2(string pathName, int interactionIndex) {
            return interactionJson.Paths.GetValueOrDefault(pathName)?.ElementAtOrDefault(interactionIndex);
        }

        public bool ProcessInteractionConditions(string pathName, int interactionIndex) {
            InteractionNode currNode = interactionJson.Paths.GetValueOrDefault(pathName)?.ElementAtOrDefault(interactionIndex);
            if (currNode == null) {
                return true;
            }
            return currNode.IncomingConditions.All(kv => AssertCondition(kv.Key, kv.Value));
        }

        public string ProcessInteractionOption(
            string pathName,
            int interactionIndex,
            int optionIndex
        ) {

            InteractionNode currNode = interactionJson.Paths.GetValueOrDefault(pathName)?.ElementAtOrDefault(interactionIndex);
            InteractionNode.InteractionOption selectedOption = currNode.Options[optionIndex];
            return selectedOption.Conditions.All(kv => AssertCondition(kv.Key, kv.Value)) ? selectedOption.NextPath : selectedOption.FallbackPath;
        }
    }
}
