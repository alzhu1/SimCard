using System.Collections;
using System.Collections.Generic;

namespace SimCard.CardGame {
    public class OpponentThinkState : OpponentState {
        protected override void Enter() {
            // Reset actions upon starting state
            opponentDuelist.AI.ResetActions();
        }

        protected override IEnumerator Handle() {
            OpponentAI opponentAI = opponentDuelist.AI;

            // Generate main action, then secondary actions based on main
            yield return opponentAI.DetermineMainAction();
            yield return opponentAI.DetermineSecondaryActions();

            // At this point, we can hand it off to the doer state
            nextState = new OpponentDoState(opponentAI.MainAction, opponentAI.SecondaryActions);
        }

        protected override void Exit() { }
    }
}
