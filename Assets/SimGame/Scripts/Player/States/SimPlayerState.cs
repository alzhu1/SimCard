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

        protected override void Enter() {
            player.SimGameManager.EventBus.OnPlayerPause.Event += HandlePause;
        }

        protected override void Exit() {
            player.SimGameManager.EventBus.OnPlayerPause.Event -= HandlePause;
        }

        void HandlePause(Common.EventArgs<bool> args) {
            player.SR.enabled = !args.argument;
            nextState = new PauseState();
        }

        // Player specific
        public virtual Vector2 RBVelocity => Vector2.zero;
    }
}
