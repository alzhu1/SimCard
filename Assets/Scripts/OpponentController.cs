using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : DuelistController {
    private float t = 0;

    void Update() {
        if (currStep != DuelistSteps.INACTIVE) {
            t += Time.deltaTime;
            if (t >= 2f) {
                t = 0;
                CardGameManager.instance.TriggerNextState();
            }
        } else {
            t = 0;
        }
    }

    protected override void ReceiveStateEnter(CardGameState gameState) {
        Debug.Log($"In player, gameState = {(int)gameState}");
        if ((int)gameState >= 3) {
            currStep = (DuelistSteps)((int)gameState % 6);
            Debug.Log($"Player state = {currStep}");
        } else {
            currStep = DuelistSteps.INACTIVE;
        }
    }

    protected override void ReceiveStateExit(CardGameState gameState) {}
}
