using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CardGameManager : MonoBehaviour {
    // TODO: Migrate away from singleton in final approach
    public static CardGameManager instance = null;

    // TODO: We likely want to keep events for other non-gameplay stuff (i.e. UI)
    // But would prefer the usage to be quite minimal
    public event Action OnGameStart = delegate {};

    [SerializeField] private List<DuelistController> turnOrder;

    void Awake() {
        instance = this;

        for (int i = 0; i < turnOrder.Count; i++) {
            turnOrder[i].SetNextTurn(turnOrder[(i + 1) % turnOrder.Count]);
        }
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        OnGameStart.Invoke();

        yield return new WaitForSeconds(2f);
        turnOrder[0].StartTurn();
    }
}
