using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentDuelist : Duelist {
        [SerializeField] private float generalWaitTime = 1f;
        public float GeneralWaitTime => generalWaitTime;

        [SerializeField]
        private OpponentAI opponentAI;
        public OpponentAI AI => opponentAI;

        protected override DuelistState StartState => new DrawState<OpponentThinkState>();

        protected override void InitForGame(EventArgs args) {
            opponentAI.InitOpponentDuelist(this);

            if (args is InitCardGameArgs cardGameArgs) {
                Deck.InitFromCardMetadata(cardGameArgs.opponentDeck, cardPrefab);
            }

            for (int i = 0; i < 4; i++) {
                DrawCard();
            }
        }
    }
}
