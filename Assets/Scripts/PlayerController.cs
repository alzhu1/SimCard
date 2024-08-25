using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : DuelistController {

    protected override void InitForGame() {
        for (int i = 0 ; i < 4; i ++) {
            duelist.DrawCard();
        }
    }

    public override void StartTurn() {
        duelistState = new DrawState<PlayerBaseState>();
        duelistState.InitState(duelist, this);
        duelistState.EnterState();
    }
}
