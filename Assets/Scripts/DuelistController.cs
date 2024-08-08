using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DuelistSteps {
    INACTIVE = -1,
    DRAW = 0,
    MAIN = 1,
    END = 2
}

public abstract class DuelistController : MonoBehaviour {
    protected DuelistSteps currStep = DuelistSteps.INACTIVE;
    protected Duelist duelist;

    void Awake() {
        duelist = GetComponent<Duelist>();
    }

    void Start() {
        CardGameManager.instance.OnStateEnter += ReceiveStateEnter;
        CardGameManager.instance.OnStateExit += ReceiveStateExit;
    }

    void OnDestroy() {
        CardGameManager.instance.OnStateEnter -= ReceiveStateEnter;
        CardGameManager.instance.OnStateExit -= ReceiveStateExit;
    }

    protected abstract void ReceiveStateEnter(CardGameState gameState);
    protected abstract void ReceiveStateExit(CardGameState gameState);
}
