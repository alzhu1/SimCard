using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public abstract class DuelistState : Common.State {
        protected Duelist duelist;
        public DuelistState NextState => Completed ? (DuelistState)nextState : null;

        public virtual void Init(Duelist duelist) {
            InitActor(duelist);
            this.duelist = duelist;
        }
    }
}
