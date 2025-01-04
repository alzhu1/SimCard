using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public class CardGameEventBus : MonoBehaviour {
        private static CardGameEventBus instance = null;

        // Game Lifecycle
        public GameEvent<InitCardGameArgs> OnGameStart = new();
        public GameEvent<EventArgs<Duelist>> OnTurnStart = new();
        public GameEvent<EventArgs<Duelist, Duelist>> OnGameEnd = new();

        // Card-specific
        public GameEvent<EventArgs<CardGraphSelectable>> OnPlayerBaseHover = new();
        public GameEvent<EventArgs<CardGraphSelectable, List<PlayerCardAction>>> OnPlayerCardSelect = new();
        public GameEvent<EventArgs<Card, Effect>> OnPlayerCardEffectHover = new();

        public GameEvent<EventArgs<PreviewUIListener>> OnPlayerCardPreview = new();

        public GameEvent<EventArgs<PlayerCardAction>> OnCardActionHover = new();

        public GameEvent<EventArgs<int, int>> OnPlayerCurrencyUpdate = new();
        public GameEvent<EventArgs<int, int>> OnOpponentCurrencyUpdate = new();

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
