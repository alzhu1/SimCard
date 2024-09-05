using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public enum OpponentAIType {
        Dummy,
        Simple,
    }

    public class OpponentDuelist : Duelist {
        [SerializeField]
        private OpponentAIType opponentAIType;
        public OpponentAIType OpponentAIType => opponentAIType;

        protected override DuelistState StartState => new DrawState<OpponentThinkState>();

        protected override void InitForGame() { }
    }
}
