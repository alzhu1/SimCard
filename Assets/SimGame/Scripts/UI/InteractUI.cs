using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.SimGame {
    public class InteractUI : MonoBehaviour {
        [SerializeField]
        private Image interactArea;

        [SerializeField]
        private TextMeshProUGUI interactText;

        [SerializeField]
        private TextMeshProUGUI dialogueText;

        public InteractUIListener Parser { get; set; }

        private CanvasGroup interactPromptGroup;
        private SimGameManager simGameManager;

        private Vector2 borderMinimizedOffset;

        void Awake() {
            interactPromptGroup = GetComponent<CanvasGroup>();
            simGameManager = GetComponentInParent<SimGameManager>();

            borderMinimizedOffset = interactArea.rectTransform.offsetMax;
        }

        void Start() {
            simGameManager.EventBus.OnCanInteract.Event += DisplayInteractPrompt;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnCanInteract.Event -= DisplayInteractPrompt;
        }

        void Update() {
            if (Parser != null && Parser.CurrInteraction != null) {
                // Non-null means we should update the dialogue text based on parser status
                dialogueText.text = Parser.CurrInteraction.text;
                dialogueText.maxVisibleCharacters = Parser.MaxVisibleCharacters;
            }
        }

        void DisplayInteractPrompt(EventArgs<Interactable> args) {
            interactPromptGroup.alpha = args.argument == null ? 0 : 1;

            // Enable interact text so future appearance has it displayed
            interactText.enabled = true;
            dialogueText.enabled = false;
        }

        public Coroutine StartInteraction() {
            return StartCoroutine(AnimateInteractionWindow(true));
        }

        public Coroutine EndInteraction() {
            dialogueText.text = "";
            return StartCoroutine(AnimateInteractionWindow(false));
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
    }
}
