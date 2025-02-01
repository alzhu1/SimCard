using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SimCard.SimGame {
    public class ShopOptionUI : MonoBehaviour {
        // [SerializeField] private TextMeshProUGUI optionText;
        // [SerializeField] private
        public TextMeshProUGUI OptionText { get; private set; }
        public TextMeshProUGUI CostText { get; private set; }

        public RectTransform RectTransform { get; private set; }

        void Awake() {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            OptionText = texts[0];
            CostText = texts[1];

            RectTransform = GetComponent<RectTransform>();
        }

        public void InitText(string optionText, string costText) {
            OptionText.text = optionText;
            CostText.text = costText;
        }
    }
}
