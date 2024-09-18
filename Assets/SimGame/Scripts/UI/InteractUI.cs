using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractUI : MonoBehaviour {
        // TODO: This InteractUI class should be used for 2 things:
        //   1. Making things visible when interaction is possible (RegularState)
        //   2. Raising/lowering the border UI to start/stop the interaction (InteractState)
        // Not sure if this should also take care of keeping track of interaction updates/choices (prob not?)
        

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
