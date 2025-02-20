using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class SimGameEventBus : MonoBehaviour {
        private static SimGameEventBus instance = null;

        // Interaction events
        public GameEvent<EventArgs<Interactable>> OnCanInteract = new();
        public GameEvent<EventArgs<OptionsUIListener, List<string>>> OnDisplayInteractOptions = new();
        public GameEvent<EventArgs<OptionsUIListener, List<CardMetadata>, Player>> OnDisplayShopOptions = new();

        // Player lifecycle events
        public GameEvent<EventArgs<bool>> OnPlayerPause = new();
        public GameEvent<EventArgs> OnPlayerUnpause = new();
        public GameEvent<EventArgs<Vector3>> OnPlayerTeleport = new();

        // MINOR: This is currently the catch-all event for interaction triggered events
        // Would like a more dynamic solution via scripting, somehow
        public GameEvent<EventArgs<string, Interactable, int>> OnInteractionEvent = new();

        // Cross scene event
        public GameEvent<EventArgs> OnSubSceneLoaded = new();
        public SubSceneInitGameEvent<EventArgs<List<CardMetadata>, List<CardMetadata>>> OnCardGameInit;
        public SubSceneInitGameEvent<EventArgs<List<CardMetadata>, List<CardMetadata>>> OnDeckBuilderInit;

        public GameEvent<EventArgs<bool, int>> OnCardGameEnd = new();
        public GameEvent<EventArgs<List<CardMetadata>, List<CardMetadata>>> OnDeckBuilderEnd = new();

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            OnCardGameInit = new(OnSubSceneLoaded);
            OnDeckBuilderInit = new(OnSubSceneLoaded);
        }
    }
}
