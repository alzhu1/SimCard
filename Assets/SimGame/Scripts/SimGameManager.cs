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

        private Player player;
        private InteractUI interactUI;
        private FadeUI fadeUI;

        // MINOR: Should we move game-level properties to another object?
        private int day = 0;

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
        }

        void OnDestroy() {
            EventBus.OnInteractionEvent.Event -= HandleInteractionEvent;
            EventBus.OnCardGameEnd.Event -= HandleCardGameEndEvent;
            EventBus.OnDeckBuilderEnd.Event -= HandleDeckBuilderEndEvent;
        }

        void HandleInteractionEvent(EventArgs<Interactable, string> args) {
            switch (args.arg2) {
                case "NextDay": {
                    StartCoroutine(GoToNextDay());
                    break;
                }

                case "StartCardGame": {
                    StartCoroutine(StartCardGame(args.arg1));
                    break;
                }

                case "StartDeckBuild": {
                    StartCoroutine(StartDeckBuilder());
                    break;
                }

                default:
                    break;
            }
        }

        void HandleCardGameEndEvent(CardGameResultArgs args) {
            Debug.Log($"Result: {args.won}, gold won: {args.goldWon}");
            StartCoroutine(EndCardGame());
        }

        void HandleDeckBuilderEndEvent(EventArgs<List<CardMetadata>, List<CardMetadata>> args) {
            Debug.Log("Returned from deck edit");
            player.UpdateDeckAfterEdit(args.arg1, args.arg2);
            StartCoroutine(EndDeckBuilder());
        }

        IEnumerator GoToNextDay() {
            EventBus.OnPlayerPause.Raise(new(false));

            yield return fadeUI.FadeInOut();
            Debug.Log($"The day is {day}");
            day++;

            player.RefreshEnergy();

            EventBus.OnPlayerUnpause.Raise(new());
        }

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
            yield return StartCoroutine(LoadSubScene(1));
            EventBus.OnCardGameInit.Raise(new(player.Deck, interactable.Deck));
        }

        IEnumerator EndCardGame() {
            yield return StartCoroutine(UnloadSubScene(1));
        }

        IEnumerator StartDeckBuilder() {
            // FIXME: Need to switch to loading DeckBuild scene
            yield return StartCoroutine(LoadSubScene(2));
            EventBus.OnDeckBuilderInit.Raise(new(player.Deck, player.AvailableCards));
        }

        IEnumerator EndDeckBuilder() {
            yield return StartCoroutine(UnloadSubScene(2));
        }

        // Scene loading helpers
        IEnumerator LoadSubScene(int sceneIndex) {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

            asyncLoad.allowSceneActivation = false;
            // Disable player movement beforehand, keep sprite (will disable later)
            EventBus.OnPlayerPause.Raise(new(false));
            yield return fadeUI.FadeIn();

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            EventBus.OnPlayerPause.Raise(new(true));
            environment.SetActive(false);
            asyncLoad.allowSceneActivation = true;

            // TODO: Maybe rely on fade out for card game scene load?
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
