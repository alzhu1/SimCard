using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState : DuelistState {
    public override void EnterState() {
        // throw new System.NotImplementedException();
    }

    public override DuelistState HandleState() {
        Debug.Log("In draw state");
        duelist.DrawCard();
        switch (duelist.type) {
            case DuelistType.HUMAN:
                return new PlayerBaseState();

            case DuelistType.AI:
                return null;
        }

        return null;
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }
}
