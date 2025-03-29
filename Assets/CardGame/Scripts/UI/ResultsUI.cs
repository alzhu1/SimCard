using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimCard.Common;

namespace SimCard.CardGame {
    [System.Serializable]
    public class ResultUICard {
        public Image cardImage;
        public TextMeshProUGUI cardName;
    }

    public class ResultsUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI resultsTitleText;
        [SerializeField] private TextMeshProUGUI resultsText;

        [SerializeField] private GameObject packCardsContainer;
        [SerializeField] private ResultUICard[] boosterPackCards;

        private CanvasGroup resultsGroup;

        void Awake() {
            resultsGroup = GetComponent<CanvasGroup>();
        }

        public void DisplayResults(bool playerWon, List<CardMetadata> pack, int goldWon) {
            resultsTitleText.text = playerWon ? "You won!" : "You lost...";
            resultsText.text = playerWon ? $"You won ${goldWon} and the following cards..." : "Better luck next time!";
            packCardsContainer.SetActive(playerWon);

            // Map cards to images
            if (playerWon) {
                for (int i = 0; i < pack.Count; i++) {
                    boosterPackCards[i].cardImage.sprite = pack[i].cardSO.smallSprite;
                    boosterPackCards[i].cardName.text = pack[i].cardSO.cardName;
                }
            }

            resultsGroup.alpha = 1;
        }

        public void HideResults() {
            resultsGroup.alpha = 0;
        }
    }
}