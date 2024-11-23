using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
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

        // TODO: Should we move game-level properties to another object?
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

            player = environment.GetComponentInChildren<Player>();
            interactUI = canvasUI.GetComponentInChildren<InteractUI>();
            fadeUI = canvasUI.GetComponentInChildren<FadeUI>();
        }

        void Start() {
            EventBus.OnInteractionEvent.Event += HandleInteractionEvent;
        }

        void OnDestroy() {
            EventBus.OnInteractionEvent.Event -= HandleInteractionEvent;
        }

        void HandleInteractionEvent(Args<string> args) {
            switch (args.argument) {
                case "NextDay": {
                    StartCoroutine(GoToNextDay());
                    break;
                }

                case "StartCardGame": {
                    StartCoroutine(StartCardGame());
                    break;
                }

                // TODO: I prefer this approach but it's a bit hacky, also no info can be transferred back to the SimGame this way
                // Either go back to method-based invocation from CardGameManager,
                // Or add a way to include parameters here
                case "EndCardGame": {
                    StartCoroutine(EndCardGame());
                    break;
                }

                default:
                    break;
            }
        }

        IEnumerator GoToNextDay() {
            player.Pause();

            yield return fadeUI.FadeInOut();
            Debug.Log($"The day is {day}");
            day++;

            player.Unpause();
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

        IEnumerator StartCardGame() {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);

            asyncLoad.allowSceneActivation = false;
            yield return fadeUI.FadeIn();

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            environment.SetActive(false);
            asyncLoad.allowSceneActivation = true;

            // TODO: Maybe rely on fade out for card game scene load?
            yield return fadeUI.FadeOut();
            canvasUI.SetActive(false);
            simGameCamera.gameObject.SetActive(false);

            yield return null;
            EventBus.OnCardGameInit.Raise(new("temp"));
        }

        IEnumerator EndCardGame() {
            canvasUI.SetActive(true);
            simGameCamera.gameObject.SetActive(true);

            yield return fadeUI.FadeIn();

            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(0);
            asyncLoad.allowSceneActivation = false;

            while (asyncLoad.progress < 0.9f) {
                yield return null;
            }

            // Disable environmental objects in sim game
            environment.SetActive(true);
            asyncLoad.allowSceneActivation = true;

            yield return fadeUI.FadeOut();
        }
    }
}
