using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SimCard.CardGame {
    public class CardGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static CardGameManager instance = null;

        // Overall game lifecycle events
        public event Action OnGameStart = delegate { };
        public event Action<Duelist> OnTurnStart = delegate { };

        private Dictionary<Duelist, int> duelistWins;

        [SerializeField]
        private Duelist playerDuelist;

        [SerializeField]
        private Duelist opponentDuelist;

        private List<List<Duelist>> roundOrder;

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

            // ABBA order, then BAAB order
            roundOrder = new List<List<Duelist>>
            {
                new(
                    new Duelist[] { playerDuelist, opponentDuelist, opponentDuelist, playerDuelist }
                ),
                new(
                    new Duelist[] { opponentDuelist, playerDuelist, playerDuelist, opponentDuelist }
                ),
            };
        }

        IEnumerator Start() {
            yield return new WaitForSeconds(1f);
            OnGameStart.Invoke();

            yield return new WaitForSeconds(2f);
            PrepareRound();
            StartTurn();
        }

        void PrepareRound() {
            Debug.Log("Preparing new round");
            if (roundOrder[0].Count == 0) {
                roundOrder.RemoveAt(0);
            }

            // Add a copy of list to end of round
            // By end of func, there should be 3 lists in roundOrder
            roundOrder.Add(new List<Duelist>(roundOrder[0]));
            Assert.IsTrue(roundOrder.Count == 3);
        }

        void StartTurn() {
            Duelist currDuelist = roundOrder[0][0];
            Debug.Log($"Start turn for {currDuelist}");
            OnTurnStart.Invoke(currDuelist);
        }

        public void EndTurn() {
            Duelist currDuelist = roundOrder[0][0];
            Debug.Log($"End turn for {currDuelist}");
            roundOrder[0].RemoveAt(0);

            if (roundOrder[0].Count == 0) {
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

                // Prepare next round
                PrepareRound();
            }

            StartTurn();
        }
    }
}
