using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class OpponentAI {
        protected List<OpponentAction> actions = new List<OpponentAction>();
        public List<OpponentAction> Actions => actions;

        protected OpponentDuelist opponentDuelist;

        public void InitOpponentDuelist(OpponentDuelist opponentDuelist) {
            this.opponentDuelist = opponentDuelist;
        }

        // This method should add on to the actions
        public abstract IEnumerator Think();
    }
}
