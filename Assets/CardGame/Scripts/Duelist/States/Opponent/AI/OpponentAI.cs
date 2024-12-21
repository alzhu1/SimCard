using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class OpponentAI : ScriptableObject {
        protected List<OpponentAction> actions = new List<OpponentAction>();
        public List<OpponentAction> Actions => actions;

        protected OpponentDuelist opponentDuelist;

        public void InitOpponentDuelist(OpponentDuelist opponentDuelist) {
            this.opponentDuelist = opponentDuelist;
        }

        public Coroutine ExecuteBehavior() {
            actions.Clear();
            return opponentDuelist.StartCoroutine(Think());
        }

        // This method should add on to the actions
        protected abstract IEnumerator Think();

        // TODO: I don't really like how the effect selection works in this system
        // When opponent plays a card, they have to be able to select the effect right after
        // This means there's a back and forth between playing the card, and selecting card effects
        // Necessitates a pause between playing cards
    }
}
