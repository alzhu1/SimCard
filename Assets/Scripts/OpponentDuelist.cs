using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentDuelist : Duelist {
    protected override DuelistState StartState => new EndState();

    protected override void InitForGame() {}
}
