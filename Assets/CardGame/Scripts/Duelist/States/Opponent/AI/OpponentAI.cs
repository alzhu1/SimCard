using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SimCard.CardGame {
    public abstract class OpponentAI : ScriptableObject {
        protected List<OpponentAction> actions = new List<OpponentAction>();
        public List<OpponentAction> Actions => actions;

        protected OpponentDuelist opponentDuelist;

        public void InitOpponentDuelist(OpponentDuelist opponentDuelist) {
            this.opponentDuelist = opponentDuelist;
        }

        public Coroutine ExecuteBehavior(bool discardMode) {
            actions.Clear();
            IEnumerator routine = discardMode ? ThinkDiscard() : Think();
            return opponentDuelist.StartCoroutine(routine);
        }

        public void EndBehavior() {
            foreach (OpponentAction action in actions) {
                Assert.IsTrue(action is not EndAction);
            }
            actions.Add(new EndAction());
        }

        // These methods should add on to the actions
        protected abstract IEnumerator Think();
        protected abstract IEnumerator ThinkDiscard();

        // TODO: I don't really like how the effect selection works in this system
        // When opponent plays a card, they have to be able to select the effect right after
        // This means there's a back and forth between playing the card, and selecting card effects
        // Necessitates a pause between playing cards
    }
}
