using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class OpponentAI : ScriptableObject {
        protected OpponentMainAction mainAction = null;
        public OpponentMainAction MainAction => mainAction;

        protected List<OpponentSecondaryAction> secondaryActions = new();
        public List<OpponentSecondaryAction> SecondaryActions => secondaryActions;

        protected OpponentDuelist opponentDuelist;

        public void InitOpponentDuelist(OpponentDuelist opponentDuelist) => this.opponentDuelist = opponentDuelist;

        public void ResetActions() {
            mainAction = null;
            secondaryActions.Clear();
        }

        public Coroutine DetermineMainAction() {
            return opponentDuelist.StartCoroutine(Think());
        }

        public Coroutine DetermineSecondaryActions() {
            // Condition checks that are here are rules of the game

            switch (mainAction) {
                case PlayCardAction playCardAction: {
                    if (playCardAction.CardToSummon.NonSelfEffects.Count > 0) {
                        // Handle setting effects for the card
                        return opponentDuelist.StartCoroutine(
                            ThinkCardEffect(playCardAction.CardToSummon)
                        );
                    }
                    break;
                }

                case null: {
                    // We need to generate discard cards
                    if (opponentDuelist.Hand.Cards.Count > Duelist.MAX_HAND_CARDS) {
                        return opponentDuelist.StartCoroutine(ThinkDiscard());
                    }
                    break;
                }

                default: {
                    // Default is to add no other actions
                    break;
                }
            }

            return null;
        }

        // Main action determiner
        protected abstract IEnumerator Think();

        // Separate methods for secondary actions
        protected abstract IEnumerator ThinkCardEffect(Card card);
        protected abstract IEnumerator ThinkDiscard();

        // MINOR: what about AI that relies on certain card/effect order application? (e.g. better to play card 1, then card 2's effect on card 1)
        // One idea could be that on the start of the turn (very first OpponentThinkState (need an arg for that when coming from DrawState))
        // We clear some temporary queue in the attached OpponentAI, and try doing a bit of scouting on the general strategy by monte carlo-ing it (small simulation)
    }
}
