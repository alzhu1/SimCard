using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : DuelistController {

    public GameObject cursor;
    public float cursorSpeed = 1;

    void Update() {
        if (currStep == DuelistSteps.INACTIVE) {
            return;
        }

        if (currStep == DuelistSteps.MAIN) {
            // Can move cursor
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 position = cursor.transform.position;
            position.x += horizontal * Time.deltaTime * cursorSpeed;
            position.y += vertical * Time.deltaTime * cursorSpeed;
            cursor.transform.position = position;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            // CardGameManager.instance.TriggerNextState();

            switch (currStep) {
                case DuelistSteps.DRAW:
                    Debug.Log("Adding to hand");
                    duelist.DrawCard();
                    break;

                case DuelistSteps.MAIN:
                    duelist.PlayFirstCard();
                    break;
                
                case DuelistSteps.END:
                    break;
            }

            CardGameManager.instance.TriggerNextState();
        }
    }

    protected override void ReceiveStateEnter(CardGameState gameState) {
        Debug.Log($"In player, gameState = {(int)gameState}");
        if ((int)gameState < 0) {
            // Start, begin by drawing some cards
            for (int i = 0 ; i < 4; i ++) {
                duelist.DrawCard();
            }

        } else if ((int)gameState < 3) {
            currStep = (DuelistSteps)gameState;
            Debug.Log($"Player state = {currStep}");
        } else {
            currStep = DuelistSteps.INACTIVE;
        }
    }

    protected override void ReceiveStateExit(CardGameState gameState) {}
}
