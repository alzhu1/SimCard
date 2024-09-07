using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            // TODO: Play resource only for now
            // But need helper methods to determine if card can be played
            yield return new WaitForSeconds(opponentDuelist.GeneralWaitTime);

            foreach (Card card in opponentDuelist.Hand.Cards) {
                if (actions.Count == opponentDuelist.TurnActions) {
                    break;
                }

                if (card.IsResourceCard()) {
                    actions.Add(new PlayCardAction(card));
                }
            }

            if (actions.Count == 0) {
                actions.Add(new EndAction());
            }

            yield break;
        }
    }
}
