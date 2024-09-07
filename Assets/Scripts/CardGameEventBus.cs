using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    // TODO: Should move args/event stuff into a different file?
    public class DuelistArgs : EventArgs {
        public Duelist duelist;
        public DuelistArgs(Duelist duelist) => this.duelist = duelist;
    }

    public class CardGameEvent<T> where T: EventArgs {
        public event Action<T> Event = delegate { };
        public void Raise(T args) => Event?.Invoke(args);
    }

    public class CardGameEventBus : MonoBehaviour {
        private static CardGameEventBus instance = null;

        public CardGameEvent<EventArgs> OnGameStart = new();
        public CardGameEvent<DuelistArgs> OnTurnStart = new();
        public CardGameEvent<EventArgs> OnGameEnd = new();

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
