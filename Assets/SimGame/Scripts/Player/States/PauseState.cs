using System;
using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using UnityEngine;

namespace SimCard.SimGame {
    public class PauseState : SimPlayerState {

        protected override void Enter() {
            base.Enter();
            player.SimGameManager.EventBus.OnPlayerUnpause.Event += HandleUnpause;
        }

        protected override void Exit() {
            base.Exit();
            player.SimGameManager.EventBus.OnPlayerUnpause.Event -= HandleUnpause;
        }

        protected override IEnumerator Handle() {
            while (nextState == null) {
                yield return null;
            }
        }

        void HandleUnpause(EventArgs _args) {
            nextState = new RegularState();
            player.SR.enabled = true;
        }
    }
}
