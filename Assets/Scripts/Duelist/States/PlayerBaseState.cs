using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : PlayerState {
    private Card startingCard;

    private CardGraph cardGraph;

    public PlayerBaseState() { }
    public PlayerBaseState(Card startingCard) {
        this.startingCard = startingCard;
    }

    protected override void Enter() {
        // For now, use Hand as holder
        // Initialize list of cards
        cardGraph = new CardGraph(new() {
            duelist.Hand.Cards
        }, this.startingCard);
    }

    protected override void Exit() {
        cardGraph.Exit();
    }

    protected override IEnumerator Handle() {
        // TODO: For now input can be put here (maybe that's ok?)
        // But think about where we could place player input and somehow merge that with DuelistController

        while (nextState == null) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                nextState = new EndState();
                break;
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                // Move to a new state
                nextState = new PlayerCardSelectedState(this.cardGraph.CurrCard);
                break;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                cardGraph.MoveNode(Vector2Int.left);
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                cardGraph.MoveNode(Vector2Int.right);
            }

            yield return null;
        }
    }
}
