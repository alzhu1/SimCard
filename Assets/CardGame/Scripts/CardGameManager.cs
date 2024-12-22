using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using SimCard.SimGame;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace SimCard.CardGame {
    public class CardGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static CardGameManager instance = null;

        // Overall game lifecycle events
        public CardGameEventBus EventBus { get; private set; }

        public CardPool CardPool { get; private set; }

        [SerializeField] private AudioListener audioListener;
        [SerializeField] private EventSystem eventSystem;

        [SerializeField]
        private Duelist playerDuelist;

        [SerializeField]
        private Duelist opponentDuelist;

        // From scene load
        private SimGameManager simGameManager;

        private Duelist[] duelistTurnOrder;

        private int currTurn;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            // If loaded additively by SimGame scene, set the SimGameManager here
            simGameManager = FindAnyObjectByType<SimGameManager>();
            if (simGameManager == null) {
                audioListener.enabled = true;
                eventSystem.enabled = true;
            }

            EventBus = GetComponent<CardGameEventBus>();
            CardPool = GetComponentInChildren<CardPool>();

            // TODO: Create mechanism to determine who goes first/second
            duelistTurnOrder = new Duelist[]{
                playerDuelist,
                opponentDuelist
            };
            currTurn = 0;

            // Second player advantage
            duelistTurnOrder[1].AdjustCurrency(25);
        }

        void Start() {
            EventBus.OnGameEnd.Event += HandleGameEnd;

            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameInit.Event += InitCardGame;
                return;
            }

            StartCoroutine(StartCardGame(null));
        }

        void OnDestroy() {
            EventBus.OnGameEnd.Event -= HandleGameEnd;

            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameInit.Event -= InitCardGame;
            }
        }

        IEnumerator StartCardGame(InitCardGameArgs args) {
            yield return new WaitForSeconds(1f);
            EventBus.OnGameStart.Raise(args);

            yield return new WaitForSeconds(2f);
            StartTurn();
        }

        void StartTurn() {
            Duelist currDuelist = duelistTurnOrder[currTurn % 2];
            Debug.Log($"Start turn for {currDuelist}");
            EventBus.OnTurnStart.Raise(new(currDuelist));
        }

        public void EndTurn() {
            Duelist currDuelist = duelistTurnOrder[currTurn % 2];
            Debug.Log($"End turn for {currDuelist}");

            currTurn++;

            if (currTurn % 4 == 0) {
                foreach (Duelist duelist in duelistTurnOrder) {
                    duelist.AdjustTaxes(1);
                }
            }

            StartTurn();
        }

        void HandleGameEnd(EventArgs<Duelist, Duelist> args) {
            Duelist winner = args.arg1;
            Duelist loser = args.arg2;

            StartCoroutine(EndCardGame(winner == playerDuelist));
        }

        IEnumerator EndCardGame(bool playerWon) {
            while (!Input.GetKeyDown(KeyCode.Return)) {
                Debug.Log("KeyDown return please");
                yield return null;
            }

            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameEnd.Raise(new(playerWon));
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void InitCardGame(InitCardGameArgs args) {
            StartCoroutine(StartCardGame(args));
            // StartCoroutine(EndCardGame(true));
        }
    }
}
