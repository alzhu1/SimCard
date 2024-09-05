using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class OpponentState : DuelistState {
        protected OpponentDuelist opponentDuelist;

        public override void Init(Duelist duelist) {
            this.duelist = duelist;
            this.opponentDuelist = (OpponentDuelist)duelist;
        }
    }
}
