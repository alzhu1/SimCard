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
        public GameEvent<InteractArgs> OnCanInteract = new();
        public GameEvent<Args<InteractionParserUIListener>> OnStartInteract = new();
        public GameEvent<Args<List<string>>> OnDisplayInteractOptions = new();
        public GameEvent<EventArgs> OnEndInteract = new();

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
