using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuelistController : MonoBehaviour {
    protected Duelist duelist;
    protected DuelistState duelistState;

    public int TotalPower => duelist.TotalPower;

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

    public void EndTurn() {
        duelistState = null;

        // TODO: There is a long chain of EndTurn calls
        // EndState -> DuelistState -> DuelistController -> CardGameManager
        // Should simplify
        CardGameManager.instance.EndTurn();
    }

    protected abstract void InitForGame();
    public abstract void StartTurn();
}
