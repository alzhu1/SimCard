using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentDoState : OpponentState {
        private List<OpponentAction> actions;

        public OpponentDoState(List<OpponentAction> actions) => this.actions = actions;

        protected override void Enter() { }

        protected override IEnumerator Handle() {
            foreach (OpponentAction action in actions) {
                switch (action) {
                    case PlayCardAction playCardAction:
                        Debug.Log("PlayCardAction");
                        break;

                    case EndAction endAction:
                        Debug.Log("EndAction");
                        nextState = new EndState();
                        break;
                }

                yield return new WaitForSeconds(1f);
            }

            if (nextState == null) {
                // Init Think state given the current AI
                nextState = new OpponentThinkState();
            }
        }

        protected override void Exit() { }
    }
}
