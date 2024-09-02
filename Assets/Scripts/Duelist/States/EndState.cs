using System.Collections;
using System.Collections.Generic;

public class EndState : DuelistState {
    protected override void Enter() { }

    protected override void Exit() { }

    protected override IEnumerator Handle() {
        duelist.EndTurn();
        yield break;
    }
}
