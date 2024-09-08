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

    public class CardArgs: EventArgs {
        public Card card;
        public CardArgs(Card card) => this.card = card;

        // TODO: Hacky, think of alternative
        // Maybe have args based on DuelistState?
        public bool isSummonAllowed;
        public CardArgs WithSummonAllowed(bool isSummonAllowed) {
            this.isSummonAllowed = isSummonAllowed;
            return this;
        }
    }

    // TODO: Would prefer not to expose the details of "icon" to gameplay
    public class IconArgs: EventArgs {
        public string iconName;
        public IconArgs(string iconName) => this.iconName = iconName;
    }

    public class CardGameEvent<T> where T: EventArgs {
        public event Action<T> Event = delegate { };
        public void Raise(T args) => Event?.Invoke(args);
    }

    public class CardGameEventBus : MonoBehaviour {
        private static CardGameEventBus instance = null;

        // Game Lifecycle
        public CardGameEvent<EventArgs> OnGameStart = new();
        public CardGameEvent<DuelistArgs> OnTurnStart = new();
        public CardGameEvent<EventArgs> OnGameEnd = new();

        // Card-specific
        public CardGameEvent<CardArgs> OnPlayerCardHover = new();
        public CardGameEvent<CardArgs> OnPlayerCardSelect = new();
        public CardGameEvent<IconArgs> OnCardIconHover = new();

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
