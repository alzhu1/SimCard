using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.DeckBuilder {
    public class DeckBuilderCardRowUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI cardNameText;

        [SerializeField]
        private TextMeshProUGUI costText;

        [SerializeField]
        private TextMeshProUGUI incomeText;

        [SerializeField]
        private TextMeshProUGUI lifetimeText;

        [SerializeField]
        private TextMeshProUGUI deckCountText;

        public RectTransform rectTransform => transform as RectTransform;

        public TextMeshProUGUI CardNameText => cardNameText;
        public TextMeshProUGUI CostText => costText;
        public TextMeshProUGUI IncomeText => incomeText;
        public TextMeshProUGUI LifetimeText => lifetimeText;
        public TextMeshProUGUI DeckCountText => deckCountText;
    }
}
