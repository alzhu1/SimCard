using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.CardGame {
    public interface OpponentAction { }

    public struct PlayCardAction : OpponentAction {
        public Card CardToSummon { get; private set; }

        public PlayCardAction(Card cardToSummon) => CardToSummon = cardToSummon;
    }

    public struct ApplyEffectAction : OpponentAction {
        public Effect Effect { get; private set; }
        public Card Source { get; private set; }
        public Card Target { get; private set; }

        public ApplyEffectAction(Effect effect, Card source, Card target) {
            Effect = effect;
            Source = source;
            Target = target;
        }
    }

    public struct DiscardAction : OpponentAction {
        public Card CardToDiscard { get; private set; }

        public DiscardAction(Card cardToDiscard) => CardToDiscard = cardToDiscard;
    }

    public struct EndAction : OpponentAction { }
}
