using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : DuelistState {
    public override void EnterState() {
        // throw new System.NotImplementedException();
    }

    public override DuelistState HandleState() {
        Debug.Log("In end state");

        // TODO: Move this somewhere else
        controller.EndTurn();

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }
}
