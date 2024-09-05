using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentDuelist : Duelist {
        [SerializeField]
        private OpponentAI opponentAI;
        public OpponentAI AI => opponentAI;

        protected override DuelistState StartState => new DrawState<OpponentThinkState>();

        protected override void InitForGame() {
            opponentAI.InitOpponentDuelist(this);
        }
    }
}
