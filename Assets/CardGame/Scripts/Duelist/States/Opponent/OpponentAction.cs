using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    // Separate based on main vs secondary
    public interface OpponentMainAction { }

    public interface OpponentSecondaryAction { }

    /* Main Actions */
    public struct PlayCardAction : OpponentMainAction {
        public Card CardToSummon { get; private set; }

        public PlayCardAction(Card cardToSummon) => CardToSummon = cardToSummon;
    }

    /* Secondary Actions */
    public struct ApplyEffectAction : OpponentSecondaryAction {
        public Effect Effect { get; private set; }
        public Card Source { get; private set; }
        public Card Target { get; private set; }

        public ApplyEffectAction(Effect effect, Card source, Card target) {
            Effect = effect;
            Source = source;
            Target = target;
        }
    }

    public struct DiscardAction : OpponentSecondaryAction {
        public Card CardToDiscard { get; private set; }

        public DiscardAction(Card cardToDiscard) => CardToDiscard = cardToDiscard;
    }
}
