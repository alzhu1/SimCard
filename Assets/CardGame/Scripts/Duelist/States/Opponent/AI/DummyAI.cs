using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    /// <summary>
    /// Dummy AI for Opponent
    ///
    /// <br />
    /// This AI just ends the turn immediately.
    /// </summary>
    [CreateAssetMenu(fileName = "DummyAI", menuName = "ScriptableObjects/AI/DummyAI")]
    public class DummyAI : OpponentAI {
        protected override IEnumerator Think() {
            // Should take no explicit actions
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);
        }

        protected override IEnumerator ThinkDiscard() {
            // For differences, let's always pick the last item in cards
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            for (int i = opponentDuelist.Hand.Cards.Count - 1; i >= Duelist.MAX_HAND_CARDS; i--) {
                actions.Add(new DiscardAction(opponentDuelist.Hand.Cards[i]));
            }
        }
    }
}
