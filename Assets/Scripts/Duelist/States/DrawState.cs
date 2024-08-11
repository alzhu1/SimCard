using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState<T> : DuelistState where T: DuelistState, new() {
    public override void EnterState() {
        // throw new System.NotImplementedException();
    }

    public override DuelistState HandleState() {
        Debug.Log("In draw state");
        duelist.DrawCard();
        return new T();
    }

    public override void ExitState() {
        // throw new System.NotImplementedException();
    }
}
