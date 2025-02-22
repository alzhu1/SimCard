using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.SimGame {
    // Presenter interface for UI to work with
    // This allows us to limit the methods it will work with
    public interface InteractUIListener {
        public int MaxVisibleCharacters { get; }

        public string CurrInteractionText { get; }
    }

    public class InteractUI : MonoBehaviour {
        [SerializeField]
        private Image interactArea;

        [SerializeField]
        private TextMeshProUGUI interactText;

        [SerializeField]
        private TextMeshProUGUI dialogueText;

        [SerializeField] private float windowTransitionTime = 1f;
        [SerializeField] private float changeRateMultiplier = 1.1f;

        public InteractUIListener Parser { get; set; }

        private CanvasGroup interactPromptGroup;
        private SimGameManager simGameManager;

        void Awake() {
            interactPromptGroup = GetComponent<CanvasGroup>();
            simGameManager = GetComponentInParent<SimGameManager>();
        }

        void Update() {
            if (Parser != null && Parser.CurrInteractionText != null) {
                // Non-null means we should update the dialogue text based on parser status
                dialogueText.text = Parser.CurrInteractionText;
                dialogueText.maxVisibleCharacters = Parser.MaxVisibleCharacters;
            }
        }

        public void Hide() {
            interactPromptGroup.alpha = 0;
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
                interactPromptGroup.alpha = 1;
                interactText.enabled = false;
                dialogueText.enabled = true;
            }

            Color whiteClear = new Color(1, 1, 1, 0);
            Color startColor = start ? whiteClear : Color.white;
            Color endColor = start ? Color.white : whiteClear;

            float t = 0;
            while (t < windowTransitionTime) {
                interactArea.color = Color.Lerp(startColor, endColor, t * changeRateMultiplier / windowTransitionTime);

                yield return null;
                t += Time.deltaTime;
            }

            interactArea.color = endColor;

            if (!start) {
                interactPromptGroup.alpha = 0;
                interactText.enabled = true;
                dialogueText.enabled = false;
            }
        }
    }
}
