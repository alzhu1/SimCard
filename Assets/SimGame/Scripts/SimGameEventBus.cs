using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;
using InteractionParserUIListener = SimCard.SimGame.InteractionParser.InteractionParserUIListener;

namespace SimCard.SimGame {
    public class SimGameEventBus : MonoBehaviour {
        private static SimGameEventBus instance = null;

        // Interaction events
        public GameEvent<Args<Interactable>> OnCanInteract = new();
        public GameEvent<Args<List<string>>> OnDisplayInteractOptions = new();

        // TODO: This is currently the catch-all event for interaction triggered events
        // Would like a more dynamic solution via scripting, somehow
        public GameEvent<Args<string>> OnInteractionEvent = new();

        // Cross scene event
        public GameEvent<Args<string>> OnCardGameInit = new();

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
