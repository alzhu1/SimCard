using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using State = SimCard.Common.State;

namespace SimCard.CardGame {
    public class OpponentDuelist : Duelist {
        [SerializeField] private float generalWaitTime = 1f;
        public float GeneralWaitTime => generalWaitTime;

        [SerializeField]
        private OpponentAI opponentAI;
        public OpponentAI AI => opponentAI;

        protected override DuelistState StartState => new DrawState<OpponentThinkState>();

        protected override void InitForGame(EventArgs _args) {
            opponentAI.InitOpponentDuelist(this);

            for (int i = 0; i < 4; i++) {
                DrawCard();
            }
        }
    }
}
