using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentDoState : OpponentState {
        public OpponentMainAction mainAction;
        public List<OpponentSecondaryAction> secondaryActions;

        public OpponentDoState(
            OpponentMainAction mainAction,
            List<OpponentSecondaryAction> secondaryActions
        ) => (this.mainAction, this.secondaryActions) = (mainAction, secondaryActions);

        protected override void Enter() { }

        protected override IEnumerator Handle() {
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            // First, handle the main action
            switch (mainAction) {
                case PlayCardAction playCardAction:
                    Debug.Log("PlayCardAction");
                    opponentDuelist.PlaySelectedCard(playCardAction.CardToSummon);
                    opponentDuelist.CardGameManager.EventBus.OnOpponentCardSummon.Raise(
                        new(playCardAction.CardToSummon)
                    );
                    break;

                // No defined action = end turn
                case null:
                    nextState = new EndState();
                    break;

                default:
                    Debug.Log($"Hitting default for main action: {mainAction}");
                    break;
            }

            // Then process secondary actions
            foreach (OpponentSecondaryAction secondaryAction in secondaryActions) {
                yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

                switch (secondaryAction) {
                    case ApplyEffectAction applyEffectAction:
                        Debug.Log("ApplyEffectAction");
                        opponentDuelist.ApplyCardEffect(applyEffectAction.Effect, applyEffectAction.Source, applyEffectAction.Target);
                        break;

                    case DiscardAction discardAction:
                        Debug.Log("DiscardAction");
                        opponentDuelist.Discard(discardAction.CardToDiscard);
                        break;

                    default:
                        Debug.Log($"Hitting default for secondary action: {secondaryAction}");
                        break;
                }
            }

            // Init Think state given the current AI
            nextState ??= new OpponentThinkState();
        }

        protected override void Exit() { }
    }
}
