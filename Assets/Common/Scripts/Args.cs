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
}
