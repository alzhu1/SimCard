using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.SimGame;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SimCard.CardGame {
    public class CardGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static CardGameManager instance = null;

        // Overall game lifecycle events
        public CardGameEventBus EventBus { get; private set; }

        private Dictionary<Duelist, int> duelistWins;

        [SerializeField]
        private Duelist playerDuelist;

        [SerializeField]
        private Duelist opponentDuelist;

        private List<List<Duelist>> roundOrder;

        // From scene load
        private SimGameManager simGameManager;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            // If loaded additively by SimGame scene, set the SimGameManager here
            simGameManager = FindAnyObjectByType<SimGameManager>();

            EventBus = GetComponent<CardGameEventBus>();

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

        void Start() {
            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameInit.Event += InitCardGame;
                return;
            }

            StartCoroutine(StartCardGame());
        }

        void OnDestroy() {
            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameInit.Event -= InitCardGame;
            }
        }

        IEnumerator StartCardGame() {
            yield return new WaitForSeconds(1f);
            EventBus.OnGameStart.Raise(EventArgs.Empty);

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
            EventBus.OnTurnStart.Raise(new DuelistArgs(currDuelist));
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
                        StartCoroutine(EndCardGame());
                        return;
                    }
                }

                // Prepare next round
                PrepareRound();
            }

            StartTurn();
        }

        // TODO: Not sure if this is the best place to put this?
        IEnumerator EndCardGame() {
            // For now, let's just reload the level
            EventBus.OnGameEnd.Raise(EventArgs.Empty);

            while (!Input.GetKeyDown(KeyCode.Return)) {
                Debug.Log("KeyDown return please");
                yield return null;
            }

            if (simGameManager != null) {
                simGameManager.EventBus.OnInteractionEvent.Raise(new("EndCardGame"));
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void InitCardGame(Args<string> args) {
            StartCoroutine(StartCardGame());
        }
    }
}
