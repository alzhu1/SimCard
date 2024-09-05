using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentThinkState : OpponentState {
        private OpponentAI opponentAI;

        protected override void Enter() {
            opponentAI = opponentDuelist.OpponentAIType switch {
                OpponentAIType.Dummy => new DummyAI(),
                OpponentAIType.Simple => new SimpleAI(),
                _ => new DummyAI(),
            };
            opponentAI.InitOpponentDuelist(opponentDuelist);
        }

        protected override IEnumerator Handle() {
            yield return opponentDuelist.StartCoroutine(opponentAI.Think());

            // At this point, there should be some actions
            nextState = new OpponentDoState(opponentAI.Actions);
        }

        protected override void Exit() { }
    }
}
