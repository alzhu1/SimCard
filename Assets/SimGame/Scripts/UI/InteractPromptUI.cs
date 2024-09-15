using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractPromptUI : MonoBehaviour {
        private CanvasGroup interactPromptGroup;
        private SimGameManager simGameManager;

        void Awake() {
            interactPromptGroup = GetComponent<CanvasGroup>();
            simGameManager = GetComponentInParent<SimGameManager>();
        }

        void Start() {
            simGameManager.EventBus.OnCanInteract.Event += DisplayInteractPrompt;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnCanInteract.Event -= DisplayInteractPrompt;
        }

        void DisplayInteractPrompt(InteractArgs args) {
            interactPromptGroup.alpha = args.interactable == null ? 0 : 1;
        }
    }
}
