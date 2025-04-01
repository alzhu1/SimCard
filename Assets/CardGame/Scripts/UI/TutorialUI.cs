using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SimCard.CardGame {
    public class TutorialUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI instructionText;

        private CanvasGroup tutorialGroup;

        void Awake() {
            tutorialGroup = GetComponent<CanvasGroup>();
        }

        public void ShowOnLoad() {
            tutorialGroup.alpha = 1;
            instructionText.text = "Press SPACE to continue.\nHold TAB to re-show tutorial.";
        }

        public void ShowDuringPlay() {
            tutorialGroup.alpha = 1;
            instructionText.text = "Release TAB to hide tutorial.";
        }

        public void Hide() {
            tutorialGroup.alpha = 0;
        }
    }
}