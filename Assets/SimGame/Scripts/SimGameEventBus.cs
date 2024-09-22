using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class SimGameEventBus : MonoBehaviour {
        private static SimGameEventBus instance = null;

        public GameEvent<InteractArgs> OnCanInteract = new();
        public GameEvent<Args<InteractionParser>> OnStartInteract = new();
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
