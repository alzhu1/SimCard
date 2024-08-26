using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuelistState {
    private DuelistController controller;
    private bool completed;

    protected Duelist duelist;

    protected DuelistState nextState;
    public DuelistState NextState => completed ? nextState : null;

    protected abstract void Enter();
    protected abstract IEnumerator Handle();
    protected abstract void Exit();

    public void Init(Duelist duelist, DuelistController controller) {
        this.duelist = duelist;
        this.controller = controller;
    }

    public void Begin() {
        // Seems like this is considered a bad practice?
        // Considering that these states should go with the duelist,
        // I feel like it should be fine

        // Alternative is to make the coroutine publich, and call StartCoroutine in controller
        controller.StartCoroutine(HandleWithLifecycle());
    }

    protected void EndTurn() {
        controller.EndTurn();
    }

    IEnumerator HandleWithLifecycle() {
        Enter();
        yield return controller.StartCoroutine(Handle());
        Exit();
        completed = true;
    }
}
