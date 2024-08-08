using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : DuelistController {

    void Update() {
        if (currStep == DuelistSteps.INACTIVE) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            // CardGameManager.instance.TriggerNextState();

            switch (currStep) {
                case DuelistSteps.DRAW:
                    Debug.Log("Adding to hand");
                    duelist.GetHand().AddCard(Random.Range(1, 20));
                    break;

                case DuelistSteps.MAIN:
                    Debug.Log($"Playing first card {duelist.GetHand().cards[0]}");
                    duelist.GetHand().RemoveCard(0);
                    break;
                
                case DuelistSteps.END:
                    break;
            }

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
