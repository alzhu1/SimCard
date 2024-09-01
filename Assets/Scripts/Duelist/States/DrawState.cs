using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawState<T> : DuelistState where T : DuelistState, new() {
    protected override void Enter() { }

    protected override void Exit() { }

    protected override IEnumerator Handle() {
        Debug.Log("In draw state");
        duelist.DrawCard();
        nextState = new T();

        yield break;
    }
}
