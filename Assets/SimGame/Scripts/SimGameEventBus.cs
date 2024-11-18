using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;
using InteractionParserUIListener = SimCard.SimGame.InteractionParser.InteractionParserUIListener;

namespace SimCard.SimGame {
    public class SimGameEventBus : MonoBehaviour {
        private static SimGameEventBus instance = null;

        // TODO: Thinking that we should refactor to use some as "mediator" pattern
        // Basically for anything that requires order/timing, mediator makes sense as we can control execution via coroutine
        // Specifically controlling the interactions between game systems, UI, and audio through a single entity (SimGameManager)

        // e.g. OnStartInteract and OnEndInteract would be mediated, as they involve UI animation and timing before we start displaying characters

        // Interaction events
        public GameEvent<InteractArgs> OnCanInteract = new();
        public GameEvent<Args<InteractionParserUIListener>> OnStartInteract = new();
        public GameEvent<Args<List<string>>> OnDisplayInteractOptions = new();
        public GameEvent<EventArgs> OnEndInteract = new();

        // TODO: This is currently the catch-all event for interaction triggered events
        // Would like a more dynamic solution via scripting, somehow
        public GameEvent<Args<string>> OnInteractionEvent = new();

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
