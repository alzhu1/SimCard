using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using SimCard.SimGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimCard.DeckBuilder {
    public class DeckBuilderManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static DeckBuilderManager instance = null;

        public DeckBuilderEventBus EventBus { get; private set; }

        [SerializeField]
        private AudioListener audioListener;

        [SerializeField]
        private EventSystem eventSystem;

        [SerializeField]
        private List<CardMetadata> testDeck;

        [SerializeField]
        private List<CardMetadata> testAvailableCards;

        private SimGameManager simGameManager;

        private Dictionary<CardSO, (int, int)> cardToCount;

        private bool running;

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

                // Also initialize editable deck/cards if null (means scene started by self)
                StartDeckBuilder(testDeck, testAvailableCards);
            }

            EventBus = GetComponent<DeckBuilderEventBus>();
        }

        void Start() {
            Debug.Log("Arrived in DeckBuilder");
            if (simGameManager != null) {
                simGameManager.EventBus.OnDeckBuilderInit.Event += InitDeckBuilder;
                return;
            }
        }

        void Update() {
            if (!running) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.M)) {
                Debug.Log("Increasing count of the first card");

                if (cardToCount.TryGetValue(cardToCount.First().Key, out (int, int) value)) {
                    cardToCount[cardToCount.First().Key] = (value.Item1 + 1, value.Item2);
                }
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                Debug.Log("Sending event back to sim game bus now");

                List<CardMetadata> finalDeck = cardToCount
                    .Where(x => x.Value.Item1 > 0)
                    .Select(x => new CardMetadata(x.Key, x.Value.Item1))
                    .ToList();
                List<CardMetadata> finalAvailableCards = cardToCount
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

                LogCardCount();
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                LogCardCount();
            }
        }

        void InitDeckBuilder(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            StartDeckBuilder(args.arg1, args.arg2);
        }

        void StartDeckBuilder(List<CardMetadata> deck, List<CardMetadata> availableCards) {
            cardToCount = new();

            foreach (CardMetadata deckCard in deck) {
                cardToCount.Add(deckCard.cardSO, (deckCard.count, deckCard.count));
            }

            foreach (CardMetadata availableCard in availableCards) {
                if (cardToCount.TryGetValue(availableCard.cardSO, out (int, int) value)) {
                    cardToCount[availableCard.cardSO] = (value.Item1, value.Item2 + availableCard.count);
                } else {
                    cardToCount.Add(availableCard.cardSO, (0, availableCard.count));
                }
            }

            running = true;
        }

        void LogCardCount() {
            // TODO: Remove debug logs
            foreach (var a in cardToCount) {
                Debug.Log($"Card {a.Key} has pair count {a.Value}");
            }
        }
    }
}
