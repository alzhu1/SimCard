using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuelistState {
    private bool completed;

    protected Duelist duelist;
    protected DuelistState nextState;
    public DuelistState NextState => completed ? nextState : null;

    protected abstract void Enter();
    protected abstract IEnumerator Handle();
    protected abstract void Exit();

    public void Init(Duelist duelist) {
        this.duelist = duelist;
    }

    public void Begin() {
        // Seems like this is considered a bad practice?
        // Considering that these states should go with the duelist,
        // I feel like it should be fine

        // Alternative is to make the coroutine publich, and call StartCoroutine in controller
        duelist.StartCoroutine(HandleWithLifecycle());
    }

    IEnumerator HandleWithLifecycle() {
        Debug.Log($"Entering {this}");
        Enter();
        Debug.Log($"Starting to handle {this}");
        yield return duelist.StartCoroutine(Handle());
        Debug.Log($"Exiting {this}");
        Exit();
        completed = true;
    }
}
