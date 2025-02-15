using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SimCard.SimGame {
    public class SimGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static SimGameManager instance = null;

        public SimGameEventBus EventBus { get; private set; }

        [SerializeField] private GameObject environment;
        [SerializeField] private GameObject canvasUI;

        private Camera simGameCamera;

        private bool loadingSubScene;

        private Player player;
        private InteractUI interactUI;
        private FadeUI fadeUI;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }

            EventBus = GetComponent<SimGameEventBus>();

            simGameCamera = Camera.main;

            player = GetComponentInChildren<Player>();
            interactUI = canvasUI.GetComponentInChildren<InteractUI>();
            fadeUI = canvasUI.GetComponentInChildren<FadeUI>();
        }

        void Start() {
            EventBus.OnInteractionEvent.Event += HandleInteractionEvent;
            EventBus.OnCardGameEnd.Event += HandleCardGameEndEvent;
            EventBus.OnDeckBuilderEnd.Event += HandleDeckBuilderEndEvent;
            EventBus.OnSubSceneLoaded.Event += HandleSubSceneLoadedEvent;
        }

        void OnDestroy() {
            EventBus.OnInteractionEvent.Event -= HandleInteractionEvent;
            EventBus.OnCardGameEnd.Event -= HandleCardGameEndEvent;
            EventBus.OnDeckBuilderEnd.Event -= HandleDeckBuilderEndEvent;
            EventBus.OnSubSceneLoaded.Event -= HandleSubSceneLoadedEvent;
        }

        void HandleInteractionEvent(EventArgs<string, Interactable, int> args) {
            (string eventName, Interactable interactable, int index) = args;

            Debug.Log($"Received event: {eventName}");

            switch (eventName) {
                case "StartCardGame": {
                    StartCoroutine(StartCardGame(interactable));
                    break;
                }

                case "StartDeckBuild": {
                    StartCoroutine(StartDeckBuilder());
                    break;
                }

                case "Buy": {
                    Debug.Log($"Buying, the arg index is {index}, card in deck is {interactable.Deck[index].cardSO}");

                    CardMetadata cardMetadataToBuy = interactable.Deck[index];
                    CardSO cardToBuy = cardMetadataToBuy.cardSO;
                    CardMetadata playerCardMetadata = player.Deck.Find(cardMetadata => cardMetadata.cardSO.Equals(cardToBuy));
                    if (playerCardMetadata != null) {
                        playerCardMetadata.count++;
                    } else {
                        player.Deck.Add(new CardMetadata(cardToBuy, 1));
                    }

                    player.ConsumeCurrency(cardMetadataToBuy.count);
                    break;
                }

                default:
                    break;
            }
        }

        void HandleCardGameEndEvent(EventArgs<bool, int> args) {
            (bool result, int goldWon) = args;
            Debug.Log($"Result: {result}, gold won: {goldWon}");
            player.IncreaseCurrency(goldWon);

            StartCoroutine(EndCardGame());
        }

        void HandleDeckBuilderEndEvent(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            Debug.Log("Returned from deck edit");
            (List<CardMetadata> deck, List<CardMetadata> availableCards) = args;
            player.UpdateDeckAfterEdit(deck, availableCards);
            StartCoroutine(EndDeckBuilder());
        }

        void HandleSubSceneLoadedEvent(EventArgs _) => loadingSubScene = false;

        // Interaction mediator methods
        public Coroutine StartInteractionCoroutine(InteractionParser parser) {
            return StartCoroutine(StartInteraction(parser));
        }

        public Coroutine EndInteractionCoroutine(InteractionParser parser) {
            return StartCoroutine(EndInteraction(parser));
        }

        IEnumerator StartInteraction(InteractionParser parser) {
            // Animate UI up + init parser for listening
            yield return interactUI.StartInteraction();
            interactUI.Parser = parser;
        }

        IEnumerator EndInteraction(InteractionParser parser) {
            // Don't reference parser anymore + animate UI down
            interactUI.Parser = null;
            yield return interactUI.EndInteraction();
            parser.EndInteraction();
        }

        IEnumerator StartCardGame(Interactable interactable) {
            yield return StartCoroutine(LoadSubScene(1, () => EventBus.OnCardGameInit.Raise(new(player.Deck, interactable.Deck))));
        }

        IEnumerator EndCardGame() {
            yield return StartCoroutine(UnloadSubScene(1));
        }

        IEnumerator StartDeckBuilder() {
            yield return StartCoroutine(LoadSubScene(2, () => EventBus.OnDeckBuilderInit.Raise(new(player.Deck, player.AvailableCards))));
        }

        IEnumerator EndDeckBuilder() {
            yield return StartCoroutine(UnloadSubScene(2));
        }

        // Scene loading helpers
        IEnumerator LoadSubScene(int sceneIndex, Action initAction) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            // Hide the interact UI when loading
            interactUI.Hide();

            asyncLoad.allowSceneActivation = false;
            // Disable player movement beforehand, keep sprite (will disable later)
            EventBus.OnPlayerPause.Raise(new(false));
            yield return fadeUI.FadeIn();

            loadingSubScene = true;

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            EventBus.OnPlayerPause.Raise(new(true));
            environment.SetActive(false);
            asyncLoad.allowSceneActivation = true;

            // Wait for sub scene to have loaded, then init
            yield return new WaitUntil(() => loadingSubScene);
            initAction.Invoke();
            yield return null;

            yield return fadeUI.FadeOut();
            canvasUI.SetActive(false);
            simGameCamera.gameObject.SetActive(false);
        }

        IEnumerator UnloadSubScene(int sceneIndex) {
            canvasUI.SetActive(true);
            simGameCamera.gameObject.SetActive(true);

            yield return fadeUI.FadeIn();

            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            EventBus.OnPlayerUnpause.Raise(new());
            environment.SetActive(true);
            asyncLoad.allowSceneActivation = true;

            yield return fadeUI.FadeOut();
        }
    }
}
