using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class OpponentDuelist : Duelist {
        protected override DuelistState StartState => new EndState();

        protected override void InitForGame() { }
    }
}
