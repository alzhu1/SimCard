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
        public GameEvent<EventArgs<List<string>>> OnDisplayInteractOptions = new();

        // TODO: This is currently the catch-all event for interaction triggered events
        // Would like a more dynamic solution via scripting, somehow
        public GameEvent<EventArgs<Interactable, string>> OnInteractionEvent = new();

        // Cross scene event
        public GameEvent<InitCardGameArgs> OnCardGameInit = new();
        public GameEvent<CardGameResultArgs> OnCardGameEnd = new();

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }
        }
    }
}
