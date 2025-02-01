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
                yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

                switch (action) {
                    case PlayCardAction playCardAction:
                        Debug.Log("PlayCardAction");
                        opponentDuelist.PlaySelectedCard(playCardAction.CardToSummon);
                        opponentDuelist.CardGameManager.EventBus.OnOpponentCardSummon.Raise(new(playCardAction.CardToSummon));
                        break;

                    case ApplyEffectAction applyEffectAction:
                        Debug.Log("ApplyEffectAction");
                        opponentDuelist.ApplyCardEffect(applyEffectAction.Effect, applyEffectAction.Source, applyEffectAction.Target);
                        break;

                    case DiscardAction discardAction:
                        Debug.Log("DiscardAction");
                        opponentDuelist.FireCard(discardAction.CardToDiscard);
                        break;

                    case EndAction:
                        if (opponentDuelist.Hand.Cards.Count > Duelist.MAX_HAND_CARDS) {
                            Debug.Log("Go back to think, for discard");
                            nextState = new OpponentThinkState(true);
                        } else {
                            Debug.Log("EndAction");
                            nextState = new EndState();
                        }
                        break;

                    default:
                        Debug.Log($"Hitting default, action: {action}");
                        break;
                }

                yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);
            }

            if (nextState == null) {
                // Init Think state given the current AI
                nextState = new OpponentThinkState();
            }
        }

        protected override void Exit() { }
    }
}
