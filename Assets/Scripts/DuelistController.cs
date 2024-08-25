using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuelistController : MonoBehaviour {
    protected Duelist duelist;
    protected DuelistState duelistState;

    protected DuelistController nextTurn;

    void Awake() {
        duelist = GetComponent<Duelist>();
    }

    void Start() {
        CardGameManager.instance.OnGameStart += InitForGame;
    }

    void OnDestroy() {
        CardGameManager.instance.OnGameStart -= InitForGame;
    }

    void Update() {
        if (duelistState != null) {
            // TODO: Pass in any necessary inputs here? Or maybe just this whole controller
            DuelistState nextState = duelistState.HandleState();

            if (nextState != null) {
                duelistState.ExitState();
                duelistState = nextState;
                duelistState.InitState(duelist, this);
                duelistState.EnterState();
            }
        }
    }

    public void SetNextTurn(DuelistController nextTurn) {
        this.nextTurn = nextTurn;
    }

    public void EndTurn() {
        duelistState = null;
        nextTurn.StartTurn();
    }

    protected abstract void InitForGame();
    public abstract void StartTurn();
}
