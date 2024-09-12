using System.Collections;
using System.Collections.Generic;

namespace SimCard.CardGame {
    public class EndState : DuelistState {
        protected override void Enter() { }

        protected override IEnumerator Handle() {
            yield break;
        }

        protected override void Exit() {
            duelist.EndTurn();
        }
    }
}
