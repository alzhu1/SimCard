using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class Args<T> : EventArgs {
        public T argument;

        public Args(T argument) => this.argument = argument;
    }

    public class Args<T, U>: EventArgs {
        public T arg1;
        public U arg2;

        public Args(T arg1, U arg2) => (this.arg1, this.arg2) = (arg1, arg2);
    }

    public class CardGameArgs : EventArgs {
        public List<CardMetadata> playerDeck;
        public List<CardMetadata> opponentDeck;

        public CardGameArgs(List<CardMetadata> playerDeck) => this.playerDeck = playerDeck;
        public CardGameArgs(List<CardMetadata> playerDeck, List<CardMetadata> opponentDeck) => (this.playerDeck, this.opponentDeck) = (playerDeck, opponentDeck);
    }
}
