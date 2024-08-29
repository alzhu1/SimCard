using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CardGameManager : MonoBehaviour {
    // TODO: Migrate away from singleton in final approach
    public static CardGameManager instance = null;

    // TODO: We likely want to keep events for other non-gameplay stuff (i.e. UI)
    // But would prefer the usage to be quite minimal
    public event Action OnGameStart = delegate {};

    private Dictionary<DuelistController, int> duelistWins;

    // [SerializeField] private List<DuelistController> turnOrder;
    [SerializeField] private DuelistController playerController;
    [SerializeField] private DuelistController opponentController;

    private DuelistController currController;

    void Awake() {
        instance = this;

        duelistWins = new Dictionary<DuelistController, int>();
        duelistWins.Add(playerController, 0);
        duelistWins.Add(opponentController, 0);

        currController = playerController;
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(1f);
        OnGameStart.Invoke();

        yield return new WaitForSeconds(2f);
        playerController.StartTurn();
    }

    DuelistController GetNextController() {
        return currController == playerController ? opponentController : playerController;
    }

    public void EndTurn() {
        currController = GetNextController();

        if (currController == playerController) {
            // Looped back around, winner is the highest overall power

            int playerPower = playerController.TotalPower;
            int opponentPower = opponentController.TotalPower;

            DuelistController winner;
            if (playerPower > opponentPower) {
                winner = playerController;
            } else if (playerPower < opponentPower) {
                winner = opponentController;
            } else {
                // TODO: Handle tie case
                Debug.Log("TIE");
                winner = null;
            }
            
            if (winner != null) {
                duelistWins[winner] += 1;

                if (duelistWins[winner] == 5) {
                    Debug.Log($"Winner: {winner}");
                    return;
                }
            }
        }

        currController.StartTurn();
    }
}
