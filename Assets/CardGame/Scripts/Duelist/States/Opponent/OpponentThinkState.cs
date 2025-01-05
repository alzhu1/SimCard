using System.Collections;
using System.Collections.Generic;

namespace SimCard.CardGame {
    public class OpponentThinkState : OpponentState {
        public bool discardMode;

        public OpponentThinkState() => this.discardMode = false;
        public OpponentThinkState(bool discardMode) => this.discardMode = discardMode;

        protected override void Enter() { }

        protected override IEnumerator Handle() {
            OpponentAI opponentAI = opponentDuelist.AI;
            yield return opponentAI.ExecuteBehavior(discardMode);

            opponentAI.EndBehavior();

            // At this point, there should be some actions
            nextState = new OpponentDoState(opponentAI.Actions);
        }

        protected override void Exit() { }
    }
}
