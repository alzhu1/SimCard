using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.DeckBuilder {
    public class DeckBuilderCardUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI cardNameText;

        [SerializeField]
        private TextMeshProUGUI costText;

        [SerializeField]
        private TextMeshProUGUI incomeText;

        [SerializeField]
        private TextMeshProUGUI deckCountText;

        public TextMeshProUGUI CardNameText => cardNameText;
        public TextMeshProUGUI CostText => costText;
        public TextMeshProUGUI IncomeText => incomeText;
        public TextMeshProUGUI DeckCountText => deckCountText;

        public void SetTextColor(Color color) {
            cardNameText.color = color;
            costText.color = color;
            incomeText.color = color;
            deckCountText.color = color;
        }
    }
}
