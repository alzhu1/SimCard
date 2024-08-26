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
            DuelistState nextState = duelistState.NextState;

            if (nextState != null) {
                duelistState = nextState;
                duelistState.Init(duelist, this);
                duelistState.Begin();
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
