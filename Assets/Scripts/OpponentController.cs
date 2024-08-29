using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentController : DuelistController {
    protected override DuelistState StartState => new EndState();

    protected override void InitForGame() {}
}
