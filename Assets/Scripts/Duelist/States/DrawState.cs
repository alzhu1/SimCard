using System.Collections;
using System.Collections.Generic;

public class DrawState<T> : DuelistState where T : DuelistState, new() {
    protected override void Enter() { }

    protected override void Exit() { }

    protected override IEnumerator Handle() {
        duelist.DrawCard();
        nextState = new T();

        yield break;
    }
}
