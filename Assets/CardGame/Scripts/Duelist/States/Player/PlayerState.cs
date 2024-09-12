using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class PlayerState : DuelistState {
        protected PlayerDuelist playerDuelist;

        public override void Init(Duelist duelist) {
            this.duelist = duelist;
            this.playerDuelist = (PlayerDuelist)duelist;
        }
    }
}
