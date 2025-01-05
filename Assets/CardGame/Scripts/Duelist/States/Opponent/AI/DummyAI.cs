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
            // The only action it should give is EndAction
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            actions.Add(new EndAction());
        }

        protected override IEnumerator ThinkDiscard() {
            // For differences, let's always pick the last item in cards
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            for (int i = opponentDuelist.Hand.Cards.Count - 1; i >= Duelist.MAX_HAND_CARDS; i--) {
                actions.Add(new DiscardAction(opponentDuelist.Hand.Cards[i]));
            }

            actions.Add(new EndAction());
        }
    }
}
