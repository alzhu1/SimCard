using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using SimCard.SimGame;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SimCard.DeckBuilder {
    public class DeckBuilderManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static DeckBuilderManager instance = null;

        public DeckBuilderEventBus EventBus { get; private set; }

        [SerializeField] private AudioListener audioListener;
        [SerializeField] private EventSystem eventSystem;

        private SimGameManager simGameManager;
        private List<CardMetadata> cardMetadata;

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
            }

            EventBus = GetComponent<DeckBuilderEventBus>();

            running = true;
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
                cardMetadata[0].count += 1;
            }

            if (Input.GetKeyDown(KeyCode.P)) {
                Debug.Log("Sending event back to sim game bus now");
                simGameManager.EventBus.OnDeckBuilderEnd.Raise(new(cardMetadata));
                running = false;
            }
        }

        void InitDeckBuilder(EventArgs<List<CardMetadata>> args) {
            cardMetadata = args.argument;
        }
    }
}
