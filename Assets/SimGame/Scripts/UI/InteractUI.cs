using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InteractionParserUIListener = SimCard.SimGame.InteractionParser.InteractionParserUIListener;

namespace SimCard.SimGame {
    public class InteractUI : MonoBehaviour {
        [SerializeField]
        private Image interactArea;

        [SerializeField]
        private TextMeshProUGUI interactText;

        [SerializeField]
        private TextMeshProUGUI dialogueText;

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

        void StartInteraction(Args<InteractionParserUIListener> args) {
            InteractionParserUIListener parser = args.argument;

            interactText.enabled = false;
            dialogueText.enabled = true;

            StartCoroutine(DisplayInteraction(parser));
        }

        void EndInteraction(EventArgs args) {
            interactText.enabled = true;
            dialogueText.enabled = false;
        }

        IEnumerator DisplayInteraction(InteractionParserUIListener parser) {
            Vector2 startOffsetMax = interactArea.rectTransform.offsetMax;
            Vector2 endOffsetMax = startOffsetMax + 50 * Vector2.up;

            // TODO: Standardize this
            float t = 0;
            while (t < 1) {
                interactArea.rectTransform.offsetMax = Vector2.Lerp(
                    startOffsetMax,
                    endOffsetMax,
                    t
                );
                yield return null;
                t += Time.deltaTime;
            }
            interactArea.rectTransform.offsetMax = endOffsetMax;

            parser.NotifyFromUI();

            while (parser.CurrInteraction != null) {
                dialogueText.text = parser.CurrInteraction.text;
                dialogueText.maxVisibleCharacters = parser.MaxVisibleCharacters;

                yield return null;
            }

            dialogueText.text = "";

            t = 0;
            while (t < 1) {
                interactArea.rectTransform.offsetMax = Vector2.Lerp(
                    endOffsetMax,
                    startOffsetMax,
                    t
                );
                yield return null;
                t += Time.deltaTime;
            }
            interactArea.rectTransform.offsetMax = startOffsetMax;

            parser.NotifyFromUI();
        }
    }
}
