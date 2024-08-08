using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Move this to component/class based, not enum
public enum CardGameState {
    PLAYER_DRAW = 0,
    PLAYER_MAIN = 1,
    PLAYER_END = 2,
    OPPONENT_DRAW = 3,
    OPPONENT_MAIN = 4,
    OPPONENT_END = 5
}


public class CardGameManager : MonoBehaviour {
    // TODO: Migrate away from singleton in final approach
    public static CardGameManager instance = null;

    public event Action<CardGameState> OnStateEnter = delegate {};
    public event Action<CardGameState> OnStateExit = delegate {};

    private CardGameState currState = CardGameState.PLAYER_DRAW;

    void Awake() {
        instance = this;
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        OnStateEnter.Invoke(currState);
    }

    void Update() {
        
    }

    public void TriggerNextState() {
        Debug.Log($"Exiting state {currState}");
        OnStateExit.Invoke(currState);
        int nextState = ((int)currState + 1) % 6;
        currState = (CardGameState)nextState;
        Debug.Log($"Entering state {currState}");
        OnStateEnter.Invoke(currState);
    }
}
