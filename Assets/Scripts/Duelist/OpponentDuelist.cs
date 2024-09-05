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

        protected override DuelistState StartState => new DrawState<OpponentThinkState>();

        public OpponentAI AI {
            get {
                OpponentAI opponentAI = opponentAIType switch {
                    OpponentAIType.Dummy => new DummyAI(),
                    OpponentAIType.Simple => new SimpleAI(),
                    _ => new DummyAI(),
                };
                opponentAI.InitOpponentDuelist(this);
                return opponentAI;
            }
        }

        protected override void InitForGame() { }
    }
}
