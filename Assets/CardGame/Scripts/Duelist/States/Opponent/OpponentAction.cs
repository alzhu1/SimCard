using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    // TODO: Decide if the actions need separate classes
    public interface OpponentAction { }

    public struct PlayCardAction : OpponentAction {
        public Card CardToSummon { get; private set; }

        public PlayCardAction(Card cardToSummon) => CardToSummon = cardToSummon;
    }

    public struct EndAction : OpponentAction { }
}
