using System;
using System.Collections.Generic;

namespace SimCard.Common {
    public class EventArgs<T> : EventArgs {
        public T argument;

        public EventArgs(T argument) => this.argument = argument;
    }

    public class EventArgs<T, U> : EventArgs {
        public T arg1;
        public U arg2;

        public EventArgs(T arg1, U arg2) => (this.arg1, this.arg2) = (arg1, arg2);
    }

    public class InitCardGameArgs : EventArgs {
        public List<CardMetadata> playerDeck;
        public List<CardMetadata> opponentDeck;

        public InitCardGameArgs(List<CardMetadata> playerDeck) => this.playerDeck = playerDeck;
        public InitCardGameArgs(List<CardMetadata> playerDeck, List<CardMetadata> opponentDeck) => (this.playerDeck, this.opponentDeck) = (playerDeck, opponentDeck);
    }

    // TODO: May need to refactor, instead of creating new args for it
    // e.g. creating new class specifically for the thing
    public class CardGameResultArgs : EventArgs {
        public bool won;
        public int goldWon;

        public CardGameResultArgs(bool won) : this(won, 50) { }
        public CardGameResultArgs(bool won, int goldWon) {
            this.won = won;
            this.goldWon = goldWon;
        }
    }
}
