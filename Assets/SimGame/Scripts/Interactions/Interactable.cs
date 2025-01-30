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
        private Player player;

        void Awake() {
            interactionJson = JObject.Parse(data.text).ToObject<Interaction>();
        }

        bool AssertCondition(ConditionKey condition, string parameter) {
            switch (condition) {
                case ConditionKey.Bool: {
                    return bool.Parse(parameter);
                }

                case ConditionKey.Energy: {
                    // This should always be a number
                    int energyCost = int.Parse(parameter);
                    return player.Energy <= energyCost;
                }

                case ConditionKey.PathTraversedCount: {
                    // This should always be in the format "PathName Operator Number"
                    string[] parts = parameter.Split(" ");
                    string pathName = parts[0];
                    string op = parts[1];

                    // LHS + RHS
                    int currentTraversedCount = pathTraversedCount.GetValueOrDefault(pathName);
                    int requiredTraversedCount = int.Parse(parts[2]);

                    return op switch {
                        "<" => currentTraversedCount < requiredTraversedCount,
                        "<=" => currentTraversedCount <= requiredTraversedCount,
                        ">" => currentTraversedCount > requiredTraversedCount,
                        ">=" => currentTraversedCount >= requiredTraversedCount,
                        "==" => currentTraversedCount == requiredTraversedCount,
                        "!=" => currentTraversedCount != requiredTraversedCount,
                        _ => false
                    };
                }

                default: {
                    return false;
                }
            }
        }

        public string InitInteraction(Player player) {
            this.player = player;

            foreach (InitInteraction.InitPathOptions pathOption in interactionJson.Init.PathOptions) {
                if (pathOption.Conditions.All(kv => AssertCondition(kv.Key, kv.Value))) {
                    return pathOption.NextPath;
                }
            }

            return "Default";
        }

        public void EndInteraction(IEnumerable<string> traversedPaths) {
            this.player = null;

            // Keep track of path counts
            foreach (string traversedPath in traversedPaths) {
                int pathCount = pathTraversedCount.GetValueOrDefault(traversedPath);
                pathTraversedCount[traversedPath] = pathCount + 1;
            }
        }

        public InteractionNode GetCurrentInteractionV2(string pathName, int interactionIndex) {
            if (pathName == null) {
                return null;
            }

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
