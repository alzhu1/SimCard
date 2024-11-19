using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        private CanvasGroup optionsGroup;

        [SerializeField]
        private Image optionCursor;

        private CanvasGroup interactPromptGroup;
        private SimGameManager simGameManager;

        private List<TextMeshProUGUI> optionTexts;
        private Vector2 borderMinimizedOffset;

        void Awake() {
            interactPromptGroup = GetComponent<CanvasGroup>();
            simGameManager = GetComponentInParent<SimGameManager>();

            optionTexts = optionsGroup.GetComponentsInChildren<TextMeshProUGUI>().ToList();
            borderMinimizedOffset = interactArea.rectTransform.offsetMax;
        }

        void Start() {
            simGameManager.EventBus.OnCanInteract.Event += DisplayInteractPrompt;
            simGameManager.EventBus.OnDisplayInteractOptions.Event += DisplayInteractOptions;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnCanInteract.Event -= DisplayInteractPrompt;
            simGameManager.EventBus.OnDisplayInteractOptions.Event -= DisplayInteractOptions;
        }

        void DisplayInteractPrompt(Args<Interactable> args) {
            interactPromptGroup.alpha = args.argument == null ? 0 : 1;

            // Enable interact text so future appearance has it displayed
            interactText.enabled = true;
            dialogueText.enabled = false;
        }

        public Coroutine StartInteraction() {
            return StartCoroutine(AnimateInteractionWindow(true));
        }

        public Coroutine EndInteraction() {
            return StartCoroutine(AnimateInteractionWindow(false));
        }

        public void HandleInteraction(InteractionParserUIListener parser) {
            StartCoroutine(DisplayInteraction(parser));
        }

        void DisplayInteractOptions(Args<List<string>> args) {
            List<string> options = args?.argument;
            if (options == null) {
                optionsGroup.alpha = 0;
                return;
            }

            optionsGroup.alpha = 1;
            for (int i = 0; i < optionTexts.Count; i++) {
                string option = options.ElementAtOrDefault(i);
                if (option != null) {
                    optionTexts[i].gameObject.SetActive(true);
                    optionTexts[i].text = option;
                } else {
                    optionTexts[i].gameObject.SetActive(false);
                }
            }
        }

        IEnumerator AnimateInteractionWindow(bool start) {
            if (start) {
                interactText.enabled = false;
                dialogueText.enabled = true;
            }

            Vector2 maximizedOffset = borderMinimizedOffset + 50 * Vector2.up;
            Vector2 startOffsetMax = start ? borderMinimizedOffset : maximizedOffset;
            Vector2 endOffsetMax = start ? maximizedOffset : borderMinimizedOffset;

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

            if (!start) {
                interactText.enabled = true;
                dialogueText.enabled = false;
            }
        }

        IEnumerator DisplayInteraction(InteractionParserUIListener parser) {
            while (parser.CurrInteraction != null) {
                dialogueText.text = parser.CurrInteraction.text;
                dialogueText.maxVisibleCharacters = parser.MaxVisibleCharacters;

                if (parser.CurrInteraction.options.Count > 0) {
                    Vector2 pos = optionCursor.rectTransform.anchoredPosition;
                    pos.y = optionTexts[parser.OptionIndex].rectTransform.anchoredPosition.y;
                    optionCursor.rectTransform.anchoredPosition = pos;
                }

                yield return null;
            }

            dialogueText.text = "";
        }
    }
}
