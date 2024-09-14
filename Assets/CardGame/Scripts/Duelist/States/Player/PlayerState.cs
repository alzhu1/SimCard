using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class PlayerState : DuelistState {
        protected PlayerDuelist playerDuelist;

        public override void Init(Duelist duelist) {
            base.Init(duelist);
            playerDuelist = (PlayerDuelist)duelist;
        }
    }
}
