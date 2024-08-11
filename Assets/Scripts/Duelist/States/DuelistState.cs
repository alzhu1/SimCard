using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuelistState {
    protected Duelist duelist;
    protected DuelistController controller;

    public abstract void EnterState();
    public abstract DuelistState HandleState();
    public abstract void ExitState();

    public void InitState(Duelist duelist, DuelistController controller) {
        this.duelist = duelist;
        this.controller = controller;
    }
}
