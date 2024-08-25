using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : DuelistController {

    protected override void InitForGame() {}

    public override void StartTurn() {
        duelistState = new EndState();
        duelistState.InitState(duelist, this);
        duelistState.EnterState();
    }
}
