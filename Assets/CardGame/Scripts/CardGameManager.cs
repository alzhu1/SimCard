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

        // Mediated UI
        private CoinUI coinUI;

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

            duelistTurnOrder = new Duelist[2];
            currTurn = 0;

            coinUI = GetComponentInChildren<CoinUI>();
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
            // yield return new WaitForSeconds(1f);
            yield return StartCoroutine(DetermineTurnOrder());
            EventBus.OnGameStart.Raise(args);

            yield return new WaitForSeconds(2f);
            StartTurn();
        }

        IEnumerator DetermineTurnOrder() {
            // TODO: Add the UI
            while (!Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Space to start coin flip");
                yield return null;
            }

            float coinFlipValue = Random.Range(0f, 1f);
            yield return coinUI.FlipCoin(coinFlipValue);

            Debug.Log($"Coin flip occurred, value: {coinFlipValue}");

            Duelist duelist1 = coinFlipValue < 0.5 ? playerDuelist : opponentDuelist;
            Duelist duelist2 = duelist1 == playerDuelist ? opponentDuelist : playerDuelist;

            duelistTurnOrder[0] = duelist1;
            duelistTurnOrder[1] = duelist2;

            // Second player advantage
            duelistTurnOrder[0].AdjustCurrency(50);
            duelistTurnOrder[1].AdjustCurrency(75);
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
