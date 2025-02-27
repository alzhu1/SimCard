using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using SimCard.SimGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimCard.DeckBuilder {
    public class DeckBuilderManager : MonoBehaviour, DeckBuilderUIListener {
        public enum SortOptions {
            NameAtoZ = 0,
            NameZtoA = 1,
            CostDescending = 2,
            CostAscending = 3,
            IncomeDescending = 4,
            IncomeAscending = 5,
            LifetimeDescending = 6,
            LifetimeAscending = 7
        }

        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static DeckBuilderManager instance = null;

        [SerializeField]
        private AudioListener audioListener;

        [SerializeField]
        private EventSystem eventSystem;

        [SerializeField]
        private List<CardMetadata> testDeck;

        [SerializeField]
        private List<CardMetadata> testAvailableCards;

        private SimGameManager simGameManager;
        private DeckBuilderUI deckBuilderUI;

        private bool running;
        private int sortOptionCount;

        // UI Listener fields
        public Dictionary<CardSO, (int, int)> CardToCount { get; private set; }
        public List<CardSO> SelectableCards { get; private set; }
        public CardSO SelectedCard { get; private set; }

        // Index at -1 indicates picking a sort/metadata option
        // Index at 0+ indicates a card is selected
        public int Index { get; private set; }
        public int SubIndex { get; private set; }

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            // If loaded additively by SimGame scene, set the SimGameManager here
            simGameManager = FindAnyObjectByType<SimGameManager>();
            if (simGameManager == null) {
                audioListener.enabled = true;
                eventSystem.enabled = true;
            }

            deckBuilderUI = GetComponentInChildren<DeckBuilderUI>();

            CardToCount = new();
        }

        void Start() {
            Debug.Log("Arrived in DeckBuilder");
            if (simGameManager != null) {
                simGameManager.EventBus.OnDeckBuilderInit.Event += InitDeckBuilder;
                return;
            } else {
                // Also initialize editable deck/cards if null (means scene started by self)
                StartDeckBuilder(testDeck, testAvailableCards);
            }
        }

        void OnDestroy() {
            if (simGameManager != null) {
                simGameManager.EventBus.OnDeckBuilderInit.Event -= InitDeckBuilder;
            }
        }

        void InitDeckBuilder(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            (List<CardMetadata> deck, List<CardMetadata> availableCards) = args;
            StartDeckBuilder(deck, availableCards);
        }

        void StartDeckBuilder(List<CardMetadata> deck, List<CardMetadata> availableCards) {
            // First, initialize dict with cards already in the deck. Value = num / num
            foreach (CardMetadata deckCardMetadata in deck) {
                CardToCount.Add(deckCardMetadata.cardSO, (deckCardMetadata.count, deckCardMetadata.count));
            }

            // Then add the available card counts to the deck. Value = numDeck / (numDeck + numAvailable)
            foreach (CardMetadata availableCardMetadata in availableCards) {
                if (CardToCount.TryGetValue(availableCardMetadata.cardSO, out (int, int) value)) {
                    CardToCount[availableCardMetadata.cardSO] = (
                        value.Item1,
                        value.Item2 + availableCardMetadata.count
                    );
                } else {
                    CardToCount.Add(availableCardMetadata.cardSO, (0, availableCardMetadata.count));
                }
            }

            // Generate a list of selectable cards for the UI to render
            SelectableCards = CardToCount.Select(x => x.Key).ToList();
            sortOptionCount = System.Enum.GetValues(typeof(SortOptions)).Length;

            // Initialize the UI listener after this
            deckBuilderUI.InitUIListener(this);
            running = true;
        }

        void Update() {
            if (!running) {
                return;
            }

            /* Actions */

            // Revert
            if (Input.GetKeyDown(KeyCode.Escape)) {
                SelectedCard = null;
            }

            // Selection
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (Index == -1) {
                    SortOptions currSortOption = (SortOptions)SubIndex;
                    Debug.Log($"Curr sort option: {currSortOption}, SubIndex: {SubIndex}");
                    SelectableCards.Sort((a, b) => {
                        return currSortOption switch {
                            SortOptions.NameAtoZ => a.cardName.CompareTo(b.cardName),
                            SortOptions.NameZtoA => b.cardName.CompareTo(a.cardName),
                            SortOptions.CostDescending => b.cost - a.cost,
                            SortOptions.CostAscending => a.cost - b.cost,
                            SortOptions.IncomeDescending => b.income - a.income,
                            SortOptions.IncomeAscending => a.income - b.income,
                            SortOptions.LifetimeDescending => b.turnLimit - a.turnLimit,
                            SortOptions.LifetimeAscending => a.turnLimit - b.turnLimit,
                            // Default to name comparison
                            _ => a.cardName.CompareTo(b.cardName),
                        };
                    });
                } else {
                    Debug.Log($"Card at index: {SelectableCards[Index]}");
                    SelectedCard = SelectableCards[Index];
                }
            }

            // Set deck (only if no selected card)
            if (SelectedCard == null && Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Log("Sending event back to sim game bus now");

                // Final deck is every card item that has a non-zero first int count
                List<CardMetadata> finalDeck = CardToCount
                    .Where(x => x.Value.Item1 > 0)
                    .Select(x => new CardMetadata(x.Key, x.Value.Item1))
                    .ToList();

                // Final available cards should be the remained (2nd int - 1st int)
                List<CardMetadata> finalAvailableCards = CardToCount
                    .Where(x => x.Value.Item2 > x.Value.Item1)
                    .Select(x => new CardMetadata(x.Key, x.Value.Item2 - x.Value.Item1))
                    .ToList();

                if (simGameManager != null) {
                    simGameManager.EventBus.OnDeckBuilderEnd.Raise(
                        new(finalDeck, finalAvailableCards)
                    );
                    running = false;
                } else {
                    // Continue running (test mode)
                    testDeck = finalDeck;
                    testAvailableCards = finalAvailableCards;
                }
            }

            /* Movement (allowed if no card selected) */

            if (SelectedCard == null) {
                int modifier = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 5 : 1;

                // Up and down
                int verticalDelta = 0;
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    // If we would go to the sort row from card index, clamp delta to get to 0 index instead
                    verticalDelta = (Index > 0 && Index - modifier < 0) ? -Index : -modifier;
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    // Clamp positive movement so that large jumps don't happen from sort row
                    verticalDelta = Index == -1 ? 1 : modifier;
                }
                Index = Mathf.Clamp(Index + verticalDelta, -1, SelectableCards.Count - 1);

                // Left and right
                int horizontalDelta = 0;
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    horizontalDelta = -modifier;
                } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    horizontalDelta = modifier;
                }

                if (Index == -1) {
                    SubIndex = (SubIndex + horizontalDelta + sortOptionCount) % sortOptionCount;
                } else {
                    CardSO currCard = SelectableCards[Index];
                    (int currValue, int totalValue) = CardToCount[currCard];
                    CardToCount[currCard] = (Mathf.Clamp(currValue + horizontalDelta, 0, totalValue), totalValue);
                }
            }
        }
    }
}
