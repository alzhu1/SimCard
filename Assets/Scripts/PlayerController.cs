using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : DuelistController {

    void Update() {
        if (currStep != DuelistSteps.INACTIVE && Input.GetKeyDown(KeyCode.Space)) {
            CardGameManager.instance.TriggerNextState();
        }
    }

    protected override void ReceiveStateEnter(CardGameState gameState) {
        Debug.Log($"In player, gameState = {(int)gameState}");
        if ((int)gameState < 3) {
            currStep = (DuelistSteps)gameState;
            Debug.Log($"Player state = {currStep}");
        } else {
            currStep = DuelistSteps.INACTIVE;
        }
    }

    protected override void ReceiveStateExit(CardGameState gameState) {}
}
