using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class OpponentState : DuelistState {
        protected OpponentDuelist opponentDuelist;

        public override void Init(Duelist duelist) {
            base.Init(duelist);
            opponentDuelist = (OpponentDuelist)duelist;
        }
    }
}
