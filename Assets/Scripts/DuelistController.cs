using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DuelistController : MonoBehaviour {
    protected Duelist duelist;
    protected DuelistState duelistState;

    void Awake() {
        duelist = GetComponent<Duelist>();
        SetDuelistType();
    }

    void Start() {
        CardGameManager.instance.OnStateEnter += ReceiveStateEnter;
        CardGameManager.instance.OnStateExit += ReceiveStateExit;
    }

    void OnDestroy() {
        CardGameManager.instance.OnStateEnter -= ReceiveStateEnter;
        CardGameManager.instance.OnStateExit -= ReceiveStateExit;
    }

    public void EndTurn() {
        duelistState = null;
        CardGameManager.instance.TriggerNextState();
    }

    protected abstract void SetDuelistType();

    protected abstract void ReceiveStateEnter(CardGameState gameState);
    protected abstract void ReceiveStateExit(CardGameState gameState);
}
