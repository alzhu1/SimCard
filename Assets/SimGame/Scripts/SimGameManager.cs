using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace SimCard.SimGame {
    public class SimGameManager : MonoBehaviour {
        // Singleton is private to avoid multiple instantiations (if it somehow happened)
        private static SimGameManager instance = null;

        public SimGameEventBus EventBus { get; private set; }

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

            player = GetComponentInChildren<Player>();
            interactUI = GetComponentInChildren<InteractUI>();
            fadeUI = GetComponentInChildren<FadeUI>();
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
    }
}
