using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDuelist : Duelist {
    protected override DuelistState StartState => new DrawState<PlayerBaseState>();

    protected override void InitForGame() {
        for (int i = 0; i < 4; i++) {
            DrawCard();
        }
    }
}
