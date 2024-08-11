using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Move this to component/class based, not enum
public enum CardGameState {
    START = -1,
    PLAYER = 0,
    OPPONENT = 1
}


public class CardGameManager : MonoBehaviour {
    // TODO: Migrate away from singleton in final approach
    public static CardGameManager instance = null;

    public event Action<CardGameState> OnStateEnter = delegate {};
    public event Action<CardGameState> OnStateExit = delegate {};

    private CardGameState currState = CardGameState.START;

    void Awake() {
        instance = this;
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        OnStateEnter.Invoke(currState);
        yield return new WaitForSeconds(2f);
        TriggerNextState();
    }

    void Update() {
        
    }

    public void TriggerNextState() {
        Debug.Log($"Exiting state {currState}");
        OnStateExit.Invoke(currState);
        int nextState = ((int)currState + 1) % 2;
        currState = (CardGameState)nextState;
        Debug.Log($"Entering state {currState}");
        OnStateEnter.Invoke(currState);
    }
}
