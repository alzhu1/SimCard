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

        [SerializeField]
        private int winCurrency = 200;
        public int WinCurrency => winCurrency;

        [SerializeField] private int startingIncome = 50;
        [SerializeField] private int laterStartingIncome = 75;

        [SerializeField]
        private List<CardPrizePool> cardPrizePools;

        // From scene load
        private SimGameManager simGameManager;

        private Duelist[] duelistTurnOrder;

        private int currTurn;

        // Mediated UI
        private CoinUI coinUI;

        private AudioSystem cardGameAudioSystem;

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

            cardGameAudioSystem = GetComponentInChildren<AudioSystem>();

            // Make sure the values in prize pool add up to 100
            Assert.AreApproximatelyEqual(100, cardPrizePools.Select(x => x.factor).Sum());
        }

        void Start() {
            EventBus.OnGameEnd.Event += HandleGameEnd;

            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameInit.Event += InitCardGame;
            } else {
                StartCoroutine(StartCardGame(null));
            }
        }

        void OnDestroy() {
            EventBus.OnGameEnd.Event -= HandleGameEnd;

            if (simGameManager != null) {
                simGameManager.EventBus.OnCardGameInit.Event -= InitCardGame;
            }
        }

        IEnumerator StartCardGame(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            StartCoroutine(StartBGM());

            yield return StartCoroutine(DetermineTurnOrder());
            EventBus.OnGameStart.Raise(args);

            yield return new WaitForSeconds(2f);
            StartTurn();
        }

        IEnumerator StartBGM() {
            Sound s = cardGameAudioSystem.Play("BGM_Intro");
            // MINOR: This doesn't perfectly lead into next song, figure out alternative
            while (s.source.isPlaying || s.source.time != 0) {
                yield return null;
            }
            cardGameAudioSystem.Play("BGM");
        }

        IEnumerator DetermineTurnOrder() {
            while (!Input.GetKeyDown(KeyCode.Space)) {
                Debug.Log("Space to start coin flip");
                yield return null;
            }

            float coinFlipValue = Random.Range(0f, 1f);
            yield return coinUI.FlipCoin(coinFlipValue);

            Debug.Log($"Coin flip occurred, value: {coinFlipValue}");

            Duelist duelist1 = coinFlipValue < 0.5 ? playerDuelist : opponentDuelist;
            Duelist duelist2 = duelist1 == playerDuelist ? opponentDuelist : playerDuelist;

            duelist1.SetEnemy(duelist2);
            duelist2.SetEnemy(duelist1);

            duelistTurnOrder[0] = duelist1;
            duelistTurnOrder[1] = duelist2;

            // Second player advantage
            duelistTurnOrder[0].AdjustCurrency(startingIncome);
            duelistTurnOrder[1].AdjustCurrency(laterStartingIncome);
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
            StartTurn();
        }

        void HandleGameEnd(EventArgs<Duelist, Duelist> args) {
            (Duelist winner, Duelist loser) = args;
            StartCoroutine(EndCardGame(winner == playerDuelist));
        }

        List<CardMetadata> GenerateBoosterPack() {
            List<CardMetadata> pack = new();

            for (int i = 0; i < 5; i++) {
                float roll = Random.Range(0, 100);
                int cardPrizePoolIndex = -1;
                while (roll >= 0) {
                    cardPrizePoolIndex++;
                    roll -= cardPrizePools[cardPrizePoolIndex].factor;
                }

                List<CardSO> cardPrizes = cardPrizePools[cardPrizePoolIndex].cardPrizes;
                int cardIndex = Random.Range(0, cardPrizes.Count);

                pack.Add(new CardMetadata(cardPrizes[cardIndex], 1));
            }

            return pack;
        }

        IEnumerator EndCardGame(bool playerWon) {
            // TODO: Fix this, don't want to use return
            while (!Input.GetKeyDown(KeyCode.Return)) {
                Debug.Log("KeyDown return please");
                yield return null;
            }

            if (simGameManager != null) {
                int goldWon = playerWon ? 50 : 0;
                List<CardMetadata> pack = playerWon ? GenerateBoosterPack() : null;

                simGameManager.EventBus.OnCardGameEnd.Raise(new(goldWon, pack));

                StartCoroutine(cardGameAudioSystem.FadeOut("BGM", 1f));
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        void InitCardGame(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            StartCoroutine(StartCardGame(args));
            // StartCoroutine(EndCardGame(true));
        }
    }
}
