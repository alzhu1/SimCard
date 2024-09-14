using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class CardGameEventBus : MonoBehaviour {
        private static CardGameEventBus instance = null;

        // Game Lifecycle
        public GameEvent<EventArgs> OnGameStart = new();
        public GameEvent<DuelistArgs> OnTurnStart = new();
        public GameEvent<EventArgs> OnGameEnd = new();

        // Card-specific
        public GameEvent<CardArgs> OnPlayerCardHover = new();
        public GameEvent<CardArgs> OnPlayerCardSelect = new();
        public GameEvent<CardArgs> OnPlayerCardPreview = new();

        // public GameEvent<IconArgs> OnCardIconHover = new();
        public GameEvent<PlayerCardActionArgs> OnCardActionHover = new();

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
