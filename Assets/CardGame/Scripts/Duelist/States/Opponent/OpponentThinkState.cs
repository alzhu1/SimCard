using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentThinkState : OpponentState {
        protected override void Enter() { }

        protected override IEnumerator Handle() {
            OpponentAI opponentAI = opponentDuelist.AI;
            yield return opponentAI.ExecuteBehavior();

            // At this point, there should be some actions
            nextState = new OpponentDoState(opponentAI.Actions);
        }

        protected override void Exit() { }
    }
}
