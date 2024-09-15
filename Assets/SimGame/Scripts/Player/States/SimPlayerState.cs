using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public abstract class SimPlayerState : Common.State {
        protected Player player;
        public SimPlayerState NextState => Completed ? (SimPlayerState)nextState : null;

        public virtual void Init(Player player) {
            InitActor(player);
            this.player = player;
        }

        // Player specific
        public virtual Vector2 RBVelocity => Vector2.zero;
    }
}
