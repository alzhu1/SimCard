using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimCard.CardGame {
    public class CardGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static CardGameManager instance = null;

        // TODO: We likely want to keep events for other non-gameplay stuff (i.e. UI)
        // But would prefer the usage to be quite minimal
        public event Action OnGameStart = delegate { };

        private Dictionary<Duelist, int> duelistWins;

        [SerializeField]
        private Duelist playerDuelist;

        [SerializeField]
        private Duelist opponentDuelist;

        private Duelist currDuelist;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            duelistWins = new Dictionary<Duelist, int>
            {
                { playerDuelist, 0 },
                { opponentDuelist, 0 },
            };

            currDuelist = playerDuelist;
        }

        IEnumerator Start() {
            yield return new WaitForSeconds(1f);
            OnGameStart.Invoke();

            yield return new WaitForSeconds(2f);
            playerDuelist.StartTurn();
        }

        Duelist GetNextDuelist() {
            return currDuelist == playerDuelist ? opponentDuelist : playerDuelist;
        }

        public void EndTurn() {
            currDuelist = GetNextDuelist();

            if (currDuelist == playerDuelist) {
                // Looped back around, winner is the highest overall power

                int playerPower = playerDuelist.TotalPower;
                int opponentPower = opponentDuelist.TotalPower;

                Duelist winner;
                if (playerPower > opponentPower) {
                    winner = playerDuelist;
                } else if (playerPower < opponentPower) {
                    winner = opponentDuelist;
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

            currDuelist.StartTurn();
        }
    }
}
