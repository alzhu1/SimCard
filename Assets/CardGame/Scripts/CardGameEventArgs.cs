using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class DuelistArgs : EventArgs {
        public Duelist duelist;
        public DuelistArgs(Duelist duelist) => this.duelist = duelist;
    }

    public class CardArgs : EventArgs {
        public Card card;
        public List<PlayerCardAction> allowedActions;

        public CardArgs(Card card) => (this.card, allowedActions) = (card, new());
        public CardArgs(Card card, List<PlayerCardAction> allowedActions) => (this.card, this.allowedActions) = (card, allowedActions);
    }

    public class PlayerCardActionArgs : EventArgs {
        public PlayerCardAction action;
        public PlayerCardActionArgs(PlayerCardAction action) => this.action = action;
    }
}
