using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentDuelist : Duelist {
        [SerializeField] private float generalWaitTime = 0.5f;
        public float GeneralWaitTime => generalWaitTime;

        [SerializeField]
        private OpponentAI opponentAI;
        public OpponentAI AI => opponentAI;

        protected override DuelistState StartState => new DrawState<OpponentThinkState>();

        protected override void InitForGame(InitCardGameArgs args) {
            AdjustCurrency(50);

            opponentAI.InitOpponentDuelist(this);

            if (args != null) {
                Deck.InitFromCardMetadata(args.opponentDeck, CardGameManager);
            }

            for (int i = 0; i < FirstDrawAmount; i++) {
                DrawCard();
            }
        }
    }
}
