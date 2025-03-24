using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class Interactable : MonoBehaviour {
        private static float GlobalTypeTime => 0.04f;

        [SerializeField]
        private float typeTime;
        public float TypeTime => typeTime == 0 ? GlobalTypeTime : typeTime;

        [SerializeField]
        private List<CardMetadata> deck;
        public List<CardMetadata> Deck => deck;

        [SerializeField] private TextAsset data;

        [SerializeField] private SpriteRenderer interactPopup;

        private Interaction interactionJson;
        private InteractableCharacterAnimator characterAnimator;

        // Metadata
        private Dictionary<string, int> pathTraversedCount = new();
        private Player player;

        void Awake() {
            interactionJson = JObject.Parse(data.text).ToObject<Interaction>();

            if (TryGetComponent(out InteractableCharacterAnimator animator)) {
                characterAnimator = animator;
            }
        }

        bool AssertCondition(ConditionKey condition, string parameter) {
            switch (condition) {
                case ConditionKey.Bool: {
                    return bool.Parse(parameter);
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

                case ConditionKey.Cost: {
                    // This should always be a number
                    int cardCost = int.Parse(parameter);
                    return cardCost <= player.Currency;
                }

                default: {
                    return false;
                }
            }
        }

        public string InitInteraction(Player player) {
            this.player = player;
            HandlePopupVisibility(false);

            // Change interactable sprite direction if doable. Negate player direction to get appropriate face
            if (characterAnimator != null) {
                characterAnimator.HandleDirectionChange(-player.PlayerDirection);
            }

            foreach (InitInteraction.InitPathOptions pathOption in interactionJson.Init.PathOptions) {
                if (pathOption.Conditions.All(kv => AssertCondition(kv.Key, kv.Value))) {
                    return pathOption.NextPath;
                }
            }

            // Also init shop options if "$ShopBuy" exists
            if (interactionJson.Paths.ContainsKey("$ShopBuy")) {
                // Create card options
                IEnumerable<InteractionNode.InteractionOption> cardOptions = deck.Select(card => new InteractionNode.InteractionOption() {
                    OptionText = card.cardSO.cardName,
                    NextPath = "$ShopBuyPost",
                    FallbackPath = "$ShopBuyFail",
                    Conditions = new() {
                        { ConditionKey.Cost, $"{card.cardSO.cost}" }
                    }
                });

                // Update the $ShopBuy path
                InteractionNode shopBuyNode = interactionJson.Paths["$ShopBuy"][0];
                shopBuyNode.Options.Clear();
                shopBuyNode.Options.AddRange(cardOptions);

                // Init $ShopBuyPost and $ShopBuyFail paths
                if (!interactionJson.Paths.ContainsKey("$ShopBuyPost")) {
                    interactionJson.Paths.Add("$ShopBuyPost", new() {
                        new() { Text = "Thanks for your purchase!" }
                    });
                }
                InteractionNode shopBuyPostNode = interactionJson.Paths["$ShopBuyPost"][0];
                shopBuyPostNode.Options.Clear();
                shopBuyPostNode.Options.AddRange(cardOptions);
                shopBuyPostNode.EventTriggers.Clear();
                shopBuyPostNode.EventTriggers.Add("Buy");

                if (!interactionJson.Paths.ContainsKey("$ShopBuyFail")) {
                    interactionJson.Paths.Add("$ShopBuyFail", new() {
                        new() { Text = "You don't have enough money." }
                    });
                }
                InteractionNode shopBuyFailNode = interactionJson.Paths["$ShopBuyFail"][0];
                shopBuyFailNode.Options.Clear();
                shopBuyFailNode.Options.AddRange(cardOptions);
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

            if (characterAnimator != null && characterAnimator.ShouldReturnToStart) {
                characterAnimator.ReturnToStart();
            }
        }

        public InteractionNode GetCurrentInteraction(string pathName, int interactionIndex) {
            if (pathName == null) {
                return null;
            }

            return interactionJson.Paths.GetValueOrDefault(pathName)?.ElementAtOrDefault(interactionIndex);
        }

        public bool ProcessInteractionConditions(string pathName, int interactionIndex) {
            InteractionNode currNode = GetCurrentInteraction(pathName, interactionIndex);
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
            InteractionNode.InteractionOption selectedOption = GetCurrentInteraction(pathName, interactionIndex)?.Options?[optionIndex];
            return selectedOption.Conditions.All(kv => AssertCondition(kv.Key, kv.Value)) ? selectedOption.NextPath : selectedOption.FallbackPath;
        }

        public void HandlePopupVisibility(bool showPopup) {
            interactPopup.enabled = showPopup;
        }
    }
}
