using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : DuelistController {

    // TODO: Probably best way to represent cards for selection
    // is with a graph, edges pertain to up/down/left/right
    // For now, just use a big list
    private List<Card> selectableCards = new();

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


        // if (currStep == DuelistSteps.MAIN) {
        //     int nextCard = 0;
        //     if (Input.GetKeyDown(KeyCode.LeftArrow)) {
        //         nextCard = -1;
        //     }
        //     if (Input.GetKeyDown(KeyCode.RightArrow)) {
        //         nextCard = 1;
        //     }

        //     if (nextCard != 0) {
        //         int currCardIndex = selectableCards.IndexOf(this.CurrCard);
        //         this.CurrCard = selectableCards[(currCardIndex + nextCard) % selectableCards.Count];
        //     }
        // }

        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     // CardGameManager.instance.TriggerNextState();

        //     switch (currStep) {
        //         case DuelistSteps.DRAW:
        //             Debug.Log("Adding to hand");
        //             duelist.DrawCard();

        //             selectableCards.Clear();
        //             selectableCards.AddRange(duelist.HandCards);
        //             selectableCards.AddRange(duelist.DeckCards);
        //             selectableCards.AddRange(duelist.FieldCards);
        //             this.CurrCard = selectableCards[0];
        //             break;

        //         case DuelistSteps.MAIN:
        //             // duelist.PlayFirstCard();
        //             duelist.PlaySelectedCard(currCard);
        //             this.CurrCard = null;
        //             break;
                
        //         case DuelistSteps.END:
        //             break;
        //     }

        //     CardGameManager.instance.TriggerNextState();
        // }
    }

    protected override void ReceiveStateEnter(CardGameState gameState) {
        Debug.Log($"In player, gameState = {(int)gameState}");

        switch (gameState) {
            case CardGameState.START:
                // Start, begin by drawing some cards
                for (int i = 0 ; i < 4; i ++) {
                    duelist.DrawCard();
                }
                duelistState = null;
                break;

            case CardGameState.PLAYER:
                duelistState = new DrawState<PlayerBaseState>();
                duelistState.InitState(duelist, this);
                duelistState.EnterState();
                break;
            
            default:
                duelistState = null;
                break;
        }

        // if ((int)gameState < 0) {
        //     // Start, begin by drawing some cards
        //     for (int i = 0 ; i < 4; i ++) {
        //         duelist.DrawCard();
        //     }

        // } else if ((int)gameState < 3) {
        //     currStep = (DuelistSteps)gameState;
        //     Debug.Log($"Player state = {currStep}");
        // } else {
        //     currStep = DuelistSteps.INACTIVE;
        // }
    }

    protected override void ReceiveStateExit(CardGameState gameState) {}
}
