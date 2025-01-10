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
        private DeckBuilderUI deckBuilderUI;

        private DeckBuilder deckBuilder;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            EventBus = GetComponent<DeckBuilderEventBus>();

            // If loaded additively by SimGame scene, set the SimGameManager here
            simGameManager = FindAnyObjectByType<SimGameManager>();
            if (simGameManager == null) {
                audioListener.enabled = true;
                eventSystem.enabled = true;
            }

            deckBuilderUI = GetComponentInChildren<DeckBuilderUI>();
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

        void Update() {
            if (deckBuilder == null) {
                return;
            }

            int modifier = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 5 : 1;

            // Revert action
            if (Input.GetKeyDown(KeyCode.Escape)) {
                deckBuilder.RevertSelection();
            }

            // Up and down
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                // MINOR: This could jump past the top card and to the sort row
                // Might want to clamp it
                deckBuilder.UpdateIndex(-1 * modifier);
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                deckBuilder.UpdateIndex(1 * modifier);
            }

            // Left and right
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                deckBuilder.UpdateAtIndex(-1 * modifier);
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                deckBuilder.UpdateAtIndex(1 * modifier);
            }

            // Action
            if (Input.GetKeyDown(KeyCode.Space)) {
                deckBuilder.SelectAtIndex();
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                Debug.Log("Sending event back to sim game bus now");

                (List<CardMetadata> finalDeck, List<CardMetadata> finalAvailableCards) =
                    deckBuilder.OutputDeckBuilder();
                if (simGameManager != null) {
                    simGameManager.EventBus.OnDeckBuilderEnd.Raise(
                        new(finalDeck, finalAvailableCards)
                    );
                    LogCardCount();
                    deckBuilder = null;
                } else {
                    // Continue running (test mode)
                    testDeck = finalDeck;
                    testAvailableCards = finalAvailableCards;
                    LogCardCount();
                }
            }

            if (Input.GetKeyDown(KeyCode.D)) {
                LogCardCount();
            }
        }

        void InitDeckBuilder(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            StartDeckBuilder(args.arg1, args.arg2);
        }

        void StartDeckBuilder(List<CardMetadata> deck, List<CardMetadata> availableCards) {
            deckBuilder = new DeckBuilder(deck, availableCards);
            deckBuilderUI.DeckBuilderUIListener = deckBuilder;
        }

        void LogCardCount() {
            // TODO: Remove debug logs
            Debug.Log($"Deck Builder is on index {deckBuilder.Index}");

            foreach (var a in deckBuilder.CardToCount) {
                Debug.Log($"Card {a.Key} has pair count {a.Value}");
            }
        }
    }
}
