using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : DuelistController {
    private float t = 0;

    void Update() {
        if (duelistState != null) {
            // TODO: Pass in any necessary inputs here? Or maybe just this whole controller
            DuelistState nextState = duelistState.HandleState();

            if (nextState != null) {
                duelistState.ExitState();
                duelistState = nextState;
                duelistState.InitState(duelist, this);
                duelistState.EnterState();
            }
        }
    }

    protected override void ReceiveStateEnter(CardGameState gameState) {
        // Debug.Log($"In player, gameState = {(int)gameState}");
        // if ((int)gameState >= 3) {
        //     currStep = (DuelistSteps)((int)gameState % 6);
        //     Debug.Log($"Player state = {currStep}");
        // } else {
        //     currStep = DuelistSteps.INACTIVE;
        // }
        if (gameState == CardGameState.OPPONENT) {
            Debug.Log("Opponent turn, for now just move on");
            duelistState = new EndState();
            duelistState.InitState(duelist, this);
        } else {
            duelistState = null;
        }
    }

    protected override void ReceiveStateExit(CardGameState gameState) {}
}
