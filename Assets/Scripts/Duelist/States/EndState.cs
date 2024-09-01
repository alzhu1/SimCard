using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndState : DuelistState {
    protected override void Enter() { }

    protected override void Exit() { }

    protected override IEnumerator Handle() {
        Debug.Log("In end state");
        duelist.EndTurn();
        yield break;
    }
}
