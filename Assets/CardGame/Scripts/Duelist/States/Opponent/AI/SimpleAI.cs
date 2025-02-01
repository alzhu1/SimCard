using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    /// <summary>
    /// Simple AI for Opponent
    ///
    /// <br />
    /// This AI will play the first card it can play in its hand.
    /// If no cards are playable, it will just end.
    /// </summary>
    [CreateAssetMenu(fileName = "SimpleAI", menuName = "ScriptableObjects/AI/SimpleAI")]
    public class SimpleAI : OpponentAI {
        protected override IEnumerator Think() {
            // If opponent cannot take more actions, immediately end it
            if (actions.Count == opponentDuelist.TurnActions) {
                yield break;
            }

            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            // Set the very first card action + associated effects
            foreach (Card card in opponentDuelist.Hand.Cards) {
                if (opponentDuelist.IsCardSummonAllowed(card)) {
                    actions.Add(new PlayCardAction(card));

                    if (card.NonSelfEffects.Count > 0) {
                        // For now just apply any non-self effects to self
                        foreach (Effect effect in card.NonSelfEffects) {
                            actions.Add(new ApplyEffectAction(effect, card, card));
                        }
                    }

                    break;
                }
            }
        }

        protected override IEnumerator ThinkDiscard() {
            // Pick first couple of cards to discard
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            for (int i = 0; i >= opponentDuelist.Hand.Cards.Count - Duelist.MAX_HAND_CARDS; i++) {
                actions.Add(new DiscardAction(opponentDuelist.Hand.Cards[i]));
            }
        }
    }
}
