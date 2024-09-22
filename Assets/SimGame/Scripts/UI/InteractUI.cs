using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace SimCard.SimGame {
    public class InteractUI : MonoBehaviour {
        // TODO: This InteractUI class should be used for 2 things:
        //   1. Making things visible when interaction is possible (RegularState)
        //   2. Raising/lowering the border UI to start/stop the interaction (InteractState)
        // Not sure if this should also take care of keeping track of interaction updates/choices (prob not?)
        [SerializeField] private TextMeshProUGUI interactText;
        [SerializeField] private TextMeshProUGUI dialogueText;

        private CanvasGroup interactPromptGroup;
        private SimGameManager simGameManager;

        void Awake() {
            interactPromptGroup = GetComponent<CanvasGroup>();
            simGameManager = GetComponentInParent<SimGameManager>();
        }

        void Start() {
            simGameManager.EventBus.OnCanInteract.Event += DisplayInteractPrompt;
            simGameManager.EventBus.OnStartInteract.Event += StartInteraction;
            simGameManager.EventBus.OnEndInteract.Event += EndInteraction;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnCanInteract.Event -= DisplayInteractPrompt;
            simGameManager.EventBus.OnStartInteract.Event -= StartInteraction;
            simGameManager.EventBus.OnEndInteract.Event -= EndInteraction;
        }

        void DisplayInteractPrompt(InteractArgs args) {
            interactPromptGroup.alpha = args.interactable == null ? 0 : 1;

            interactText.enabled = true;
            dialogueText.enabled = false;
        }

        void StartInteraction(Args<InteractionParser> args) {
            InteractionParser parser = args.argument;

            interactText.enabled = false;
            dialogueText.enabled = true;

            StartCoroutine(DisplayInteraction(parser));
        }

        void EndInteraction(EventArgs args) {
            interactText.enabled = true;
            dialogueText.enabled = false;
        }

        IEnumerator DisplayInteraction(InteractionParser parser) {
            Interaction currInteraction = parser.CurrInteraction;
            while (currInteraction != null) {
                dialogueText.text = currInteraction.text;
                dialogueText.maxVisibleCharacters = (int)(parser.CurrInteractionTime / currInteraction.TypeTime);

                yield return null;
                currInteraction = parser.CurrInteraction;
            }
        }
    }
}
