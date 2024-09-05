using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    // TODO: Decide if the actions need separate classes
    public interface OpponentAction { }

    public class PlayCardAction : OpponentAction {
        public Card cardToSummon;

        public PlayCardAction(Card cardToSummon) => this.cardToSummon = cardToSummon;
    }

    public class EndAction : OpponentAction { }
}
